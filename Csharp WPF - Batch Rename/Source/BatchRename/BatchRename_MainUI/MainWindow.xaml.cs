using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using Fluent;
using BatchRename_MainClasses;
using BatchRename_UtilClasses;
using BatchRename_UtilClassess;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BatchRename_MainUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        ObservableCollection<IRule> _availableRules = new ObservableCollection<IRule> { };

        Project _project = new Project()
        {
            Files = new ObservableCollection<FileItem> { },
            RuleList = new Preset
            {
                Rules = new ObservableCollection<IRule> { }
            }
        };

        public string NewLocation { get; set; } = "";
        public MainWindow()
        {
            InitializeComponent();
            SetupData();
        }

        #region "Init-Things"
        public void SetupData()
        {

            FileItemList.ItemsSource = _project.Files;
            RuleList.ItemsSource = _project.RuleList.Rules;
            ChooseRuleComboBox.ItemsSource = _availableRules;

            ReadDLL();
        }
        public void ReadDLL()
        {
            var dllUri = new Uri("library", UriKind.Relative);
            var readDLLResponse = DLLUtils
                .getAllImplementableClassInstanceFromDLLs<IRule>(
                    dllUri.ToString()
            );

            if (readDLLResponse.Item1 == true)
            {
                foreach (var item in readDLLResponse.Item3)
                {
                    var isSuccessfullyRegistered = Factory<IRule>
                        .Register(
                            item.RuleName,
                            (IRule)item.Clone()
                        );

                    if (isSuccessfullyRegistered)
                    {
                        _availableRules.Add(item);
                    }
                    else
                    {
                        // Do nothing
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show(readDLLResponse.Item2);
            }
        }
        #endregion

        #region "Resolve-Duplication"
        private string FileItemToStringKey(FileItem item)
        {
            string result = string.Concat(
                item.IsFolder, 
                item.Location, 
                item.OldName
            );

            return result;
        }
        private string NewFileItemToStringKey(
            FileItem item, 
            string newLocation = ""
        ) {
            string result = string.Concat(
                item.IsFolder,
                (newLocation == "") ? item.Location : newLocation,
                item.NewName
            );

            return result;
        }
        private int NotifyDuplicateErrorIfExistsOnAddingFileItem(
            int totalAdded,
            List<FileItem> duplicates,
            int maximumItemCountToNotify
        ) {
            string message = "";

            int nDuplicate = duplicates.Count;
            
            if (nDuplicate > maximumItemCountToNotify)
            {
                message = string.Concat(
                    $"Tried to add {totalAdded} item(s).\n",
                    "Ignored duplicated item(s):\n",
                    $"-> {nDuplicate} duplicated items was ignored"
                );

            }
            else if (nDuplicate > 0)
            {
                message = string.Concat(
                    $"Tried to add {totalAdded} item(s).\n",
                    $"Ignored {nDuplicate} duplicated item(s):\n"
                );

                foreach (var item in duplicates)
                {
                    message = string.Concat(
                        message,
                        "-> ",
                        PathUtils.FileLocationAndFileNameToFilePathConverter(
                            item.Location,
                            item.OldName
                        ),
                        "\n"
                    );
                }

            }
            else
            {
                message = $"Successfully added {totalAdded} items.";
            }

            System.Windows.MessageBox.Show(message, "Item Adding Result");

            return nDuplicate;
        }
        private int ResolveDuplicatedItemsInTheProjectOnAddingFileItem(int totalAdded)
        {
            var grouper = new Dictionary<string, List<FileItem>> { };

            foreach (var item in _project.Files)
            {
                string key = FileItemToStringKey(item);
                try
                {
                    grouper[key].Add(item);
                }
                catch
                {
                    grouper.Add(key, new List<FileItem> { item });
                }
            }

            var duplicates = new List<FileItem>(
                from item in grouper
                where item.Value.Count > 1
                select item.Value[0]
            );

            var query = _project.Files.DistinctBy(x => FileItemToStringKey(x)).ToList();

            _project.Files.Clear();
            _project.Files = new ObservableCollection<FileItem>(query);
            FileItemList.ItemsSource = _project.Files;

            int nDuplicate = NotifyDuplicateErrorIfExistsOnAddingFileItem(
                totalAdded,
                duplicates,
                maximumItemCountToNotify: 15
            );

            int nDistinctItemsAdded = totalAdded - nDuplicate;
            return nDistinctItemsAdded;
        }
        private int ResolveDuplicatedNewFullPathsBeforeExecute(
            bool shouldUseNewLocation
        ) {
            int currentIndex = 0;
            var distinctItemsCounter = new Dictionary<string, DistinctFileItemHepler> { };

            foreach (var item in _project.Files)
            {
                string stringKey = NewFileItemToStringKey(
                    item,
                    shouldUseNewLocation ? NewLocation : ""
                );

                if (distinctItemsCounter
                    .ContainsKey(stringKey)
                )
                {
                    string name = "";
                    string extension = "";

                    if (item.IsFolder)
                    {
                        name = item.NewName;
                    }
                    else
                    {
                        (name, extension) = PathUtils.FileNameToNameAndExtension(
                            item.NewName
                        );
                    }

                    item.NewName = string.Concat(
                        name,
                        $" ({distinctItemsCounter[stringKey].Count + 1}).",
                        extension
                    );

                    item.ErrorAndWarning = string.Concat(
                        item.ErrorAndWarning,
                        "A counter was added to the end of the new name due to name duplication; "
                    );

                    if (distinctItemsCounter[stringKey].Count == 0)
                    {
                        int firstVersionIndex = distinctItemsCounter[stringKey].FirstIndex;
                        _project.Files[firstVersionIndex].NewName = string.Concat(
                            name,
                            $" (0).",
                            extension
                        );

                        _project.Files[firstVersionIndex].ErrorAndWarning = string.Concat(
                            _project.Files[firstVersionIndex].ErrorAndWarning,
                            "A counter was added to the end of the new name due to name duplication; "
                        );

                        distinctItemsCounter[stringKey].Count += 1;
                    }
                    else
                    {
                        // Do nothing
                    }

                    distinctItemsCounter[stringKey].Count += 1;
                }
                else
                {
                    distinctItemsCounter.Add(
                        stringKey,
                        new DistinctFileItemHepler
                        {
                            Count = 0,
                            FirstIndex = currentIndex
                        }
                    );
                }

                currentIndex++;
            }

            int nDuplicated = distinctItemsCounter.Select(x => x.Value.Count).Sum();
            return nDuplicated;
        }
        class DistinctFileItemHepler
        {
            public int FirstIndex { get; set; } = -1;
            public int Count { get; set; } = -1;
        }

        private Tuple<string, string> FileItemToSourceAndDestinationPath(
            FileItem item,
            bool shouldUseNewLocation
        )
        {
            string sourceFileName = PathUtils
                .FileLocationAndFileNameToFilePathConverter(
                        location: item.Location,
                        filename: item.OldName
                );

            string destFileName = PathUtils
                .FileLocationAndFileNameToFilePathConverter(
                    location: (shouldUseNewLocation ? NewLocation : item.Location),
                    filename: item.NewName
                );

            return new Tuple<string, string>(sourceFileName, destFileName);
        }
        private string NewDuplicateFolder
        {
            get => string.Concat(
                   AppDomain.CurrentDomain.BaseDirectory,
                   "Duplication Recovery",
                   AppDomain.CurrentDomain.BaseDirectory.Last(),
                   DateTime.Now.ToString("yyyy MM dd HH:mm:ss tt").Replace(':', '-')
                );
        }
        #endregion

        #region "Context-Menu"
        private void CopyErrorContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = FileItemList.SelectedIndex;
                string message = _project.Files[selectedIndex].ErrorAndWarning;
                System.Windows.Clipboard.SetText(message);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void AddFileContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Select your file(s)",
                    Filter = string.Concat(
                        "Any available file|*.*"
                    ),
                    Multiselect = true
                };

                if (fileDialog.ShowDialog() == true)
                {
                    foreach (var f in fileDialog.FileNames)
                    {
                        var pathParser = PathUtils
                            .FullFilePathToLocationAndFileName(f);

                        _project.Files.Add(
                            new FileItem
                            {
                                OldName = pathParser.Item2,
                                NewName = pathParser.Item2,
                                Location = pathParser.Item1,
                                ErrorAndWarning = "",
                                IsFolder = false
                            }
                        );
                    }

                    int nDistinctItemsAdded = ResolveDuplicatedItemsInTheProjectOnAddingFileItem(
                        totalAdded: fileDialog.FileNames.Length
                    );

                    int nItem = _project.Files.Count;

                    int endFileIndex = nItem - 1;
                    int endRuleIndex = _project.RuleList.Rules.Count - 1;

                    if (endRuleIndex == -1)
                    {
                        foreach (var file in _project.Files)
                        {
                            if (!file.IsFolder)
                            {
                                file.NewName = file.OldName;
                                file.ErrorAndWarning = "";
                            }
                            else
                            {
                                // Do nothing
                            }
                        }
                    }
                    else
                    {
                        bool isResetPerformed = ResetGlobalAffectRules();

                        if (!isResetPerformed)
                        {
                            ResetErrorAndWarning();
                            ResetToOldName();

                            ApplyRulesToFileList(
                                startRuleIndex: 0,
                                endRuleIndex: _project.RuleList.Rules.Count - 1,
                                startFileIndex: 0,
                                endFileIndex: _project.Files.Count - 1
                            );
                        }
                        else
                        {
                            // Do nothing
                        }
                    }
                }
                else
                {
                    // Do nothing
                }
            }
            catch( Exception ex ) 
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void AddFolderContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonOpenFileDialog commonFileDialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true,
                    Multiselect= true,
                    Title = "Select your folder(s)"

                };

                if (commonFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (var f in commonFileDialog.FileNames)
                    {
                        var pathParser = PathUtils
                            .FullFilePathToLocationAndFileName(f);

                        _project.Files.Add(
                            new FileItem
                            {
                                OldName = pathParser.Item2,
                                NewName = pathParser.Item2,
                                Location = pathParser.Item1,
                                ErrorAndWarning = "",
                                IsFolder = true
                            }
                        );
                    }

                    int nDistinctItemsAdded = ResolveDuplicatedItemsInTheProjectOnAddingFileItem(
                        totalAdded: commonFileDialog.FileNames.Count()
                    );

                    int nItem = _project.Files.Count;

                    int startFileIndex = nItem - nDistinctItemsAdded;
                    int endFileIndex = nItem - 1;
                    int endRuleIndex = _project.RuleList.Rules.Count - 1;

                    if (endRuleIndex == -1)
                    {
                        foreach (var file in _project.Files)
                        {
                            if (file.IsFolder)
                            {
                                file.NewName = file.OldName;
                                file.ErrorAndWarning = "";
                            }
                            else
                            {
                                // Do nothing
                            }
                        }
                    }
                    else
                    {
                        bool isResetPerformed = ResetGlobalAffectRules();

                        if (!isResetPerformed)
                        {
                            ResetErrorAndWarning();
                            ResetToOldName();

                            ApplyRulesToFileList(
                                startRuleIndex: 0,
                                endRuleIndex: _project.RuleList.Rules.Count - 1,
                                startFileIndex: 0,
                                endFileIndex: _project.Files.Count - 1
                            );
                        }
                        else
                        {
                            // Do nothing
                        }
                    }
                }
                else
                {
                    // Do nothing
                }
            }
            catch(Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message);    
            }
        }
        private void DeleteFileItemContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                // Init an array with nSelected size 
                var selected = new FileItem[FileItemList.SelectedItems.Count];

                // Add selected items to the array
                FileItemList.SelectedItems.CopyTo(selected, index: 0);

                foreach (var item in selected)
                {
                    _project.Files.Remove(item);
                }

                ResetGlobalAffectRules();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void DeleteRuleContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = new IRule[RuleList.SelectedItems.Count];
                RuleList.SelectedItems.CopyTo(selected, index: 0);
                foreach (var item in selected)
                {
                    _project.RuleList.Rules.Remove(item);
                }

                bool isResetPerformed = ResetGlobalAffectRules();

                if (!isResetPerformed)
                {
                    ResetErrorAndWarning();
                    ResetToOldName();

                    ApplyRulesToFileList(
                        startRuleIndex: 0,
                        endRuleIndex: _project.RuleList.Rules.Count - 1,
                        startFileIndex: 0,
                        endFileIndex: _project.Files.Count - 1
                    );
                }
                else
                {
                    // Do nothing
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void CloneRuleContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = RuleList.SelectedIndex;
                IRule selectedRule = _project.RuleList.Rules[selectedIndex];

                bool success = true;
                string message = "";
                IRule? newRule;
                
                (success, message, newRule) = Factory<IRule>.Instance().Produce(selectedRule.RuleName);

                if (success) 
                {
                    newRule!.RuleParameters = new Dictionary<string, string>(
                        selectedRule.RuleParameters
                    );

                    _project.RuleList.Rules.Insert(
                        selectedIndex + 1,
                        newRule
                    );
                }
                else
                {
                    System.Windows.MessageBox.Show(message, "Clone Error");
                }

                bool isResetPerformed = ResetGlobalAffectRules();

                if (!isResetPerformed)
                {
                    ResetErrorAndWarning();
                    ResetToOldName();

                    ApplyRulesToFileList(
                        startRuleIndex: 0,
                        endRuleIndex: _project.RuleList.Rules.Count - 1,
                        startFileIndex: 0,
                        endFileIndex: _project.Files.Count - 1
                    );
                }
                else
                {
                    // Do nothing
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region "Rule-Buttons"
        private void DeleteRuleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = RuleList.SelectedIndex;
                _project.RuleList.Rules.RemoveAt(selectedIndex);

                ResetErrorAndWarning();
                ResetToOldName();

                ApplyRulesToFileList(
                    startRuleIndex: 0,
                    endRuleIndex: _project.RuleList.Rules.Count - 1,
                    startFileIndex: 0,
                    endFileIndex: _project.Files.Count - 1
                );
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void AddRuleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChooseRuleComboBox.SelectedIndex != -1)
            {
                int selectedIndex = ChooseRuleComboBox.SelectedIndex;
                string ruleName = _availableRules[selectedIndex].RuleName;

                bool success = true;
                string message = "";
                IRule? result;

                (success, message, result) = 
                    Factory<IRule>
                        .Instance()
                        .Produce(
                            ruleName
                        );

                if (success)
                {
                    // result!.RuleParameters = new Dictionary<string, string>(result.RuleParameters);
                    _project.RuleList.Rules.Add((IRule)result!.Clone());
                    
                    int nRules = _project.RuleList.Rules.Count;

                    var parameters = new Dictionary<string, string>(result.RuleParameters);

                    _project.RuleList.Rules[nRules - 1].RuleParameters = parameters;


                    ApplyRulesToFileList(
                        startRuleIndex: nRules - 1,
                        endRuleIndex: nRules - 1,
                        startFileIndex: 0,
                        endFileIndex: _project.Files.Count - 1
                    );
                }
                else
                {
                    System.Windows.MessageBox.Show(message, "Factory Error");
                }
                
                ChooseRuleComboBox.SelectedIndex = -1;
            }
            else
            {
                // Do nothing
            }
        }
        private void MoveRuleDownButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = RuleList.SelectedIndex;
            try
            {
                if (selectedIndex + 1 < _project.RuleList.Rules.Count)
                {
                    _project.RuleList.Rules.Swap<IRule>(
                        selectedIndex, 
                        selectedIndex + 1
                    );
                }

                RuleList.SelectedIndex = selectedIndex;

                ResetErrorAndWarning();
                ResetToOldName();

                ApplyRulesToFileList(
                    startRuleIndex: 0,
                    endRuleIndex: _project.RuleList.Rules.Count - 1,
                    startFileIndex: 0,
                    endFileIndex: _project.Files.Count - 1
                );
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void MoveRuleUpButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = RuleList.SelectedIndex;
            try
            {
                if (selectedIndex > 0)
                {
                    _project.RuleList.Rules.Swap<IRule>(selectedIndex, selectedIndex - 1);
                }
                RuleList.SelectedIndex = selectedIndex;

                ResetErrorAndWarning();
                ResetToOldName();

                ApplyRulesToFileList(
                    startRuleIndex: 0,
                    endRuleIndex: _project.RuleList.Rules.Count - 1,
                    startFileIndex: 0,
                    endFileIndex: _project.Files.Count - 1
                );
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void RuleSettingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = RuleList.SelectedIndex;
                var rule = _project.RuleList.Rules[selectedIndex];
                if (rule.ShouldAnyDialogBeOpened)
                {
                    ShowSettingDialog(selectedIndex);
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        "Selected rule has no setting dialog.",
                        "Open Rule Dialog"
                    );
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    ex.Message,
                    "Open Rule Dialog"
                );
            }
        }
        private void ReloadAvailableRuleList_Click(object sender, RoutedEventArgs e)
        {
            ReadDLL();
            System.Windows.MessageBox.Show(
                string.Concat(
                    "The number of available rule(s): ",
                    _availableRules.Count.ToString(),
                    " rule(s)"
                ),
                "Update Rule List"
            );
        }

        #endregion

        #region "Globally-Reset-or-Apply"
        private void ResetToOldName()
        {
            foreach (var item in _project.Files)
            {
                item.NewName = item.OldName;
            }
        }
        public void ApplyRulesToFileList(
            int startRuleIndex,
            int endRuleIndex,
            int startFileIndex, 
            int endFileIndex
        ) {
            bool success = true;
            string message = "";
            string newName = "";

            for (int iFile = startFileIndex; iFile <= endFileIndex; iFile++)
            {
                if (endRuleIndex - startRuleIndex < 0)
                {
                    _project.Files[iFile].NewName = _project.Files[iFile].OldName;
                }
                else
                {
                    for(int iRule = startRuleIndex; iRule <= endRuleIndex; iRule++)
                    {
                        try
                        {
                            (success, message, newName) = _project
                                .RuleList
                                .Rules[iRule]
                                .Rename(
                                    _project.Files[iFile].NewName,
                                    _project.Files[iFile].IsFolder
                                );

                            if (success)
                            {
                                _project.Files[iFile].NewName = newName;
                            }
                            else
                            {
                                _project.Files[iFile].ErrorAndWarning = string
                                    .Concat(
                                        _project.Files[iFile].ErrorAndWarning,
                                        message,
                                        "; "
                                    );
                            }
                        }
                        catch (Exception ex)
                        {
                            _project.Files[iFile].NewName = _project.Files[iFile].NewName;
                            _project.Files[iFile].ErrorAndWarning = string.Concat(
                                _project.Files[iFile].ErrorAndWarning, 
                                ex.StackTrace,
                                " -> ",
                                ex.Message,
                                "; "
                            );
                        }
                    }
                }
            }
        }
        private void ResetErrorAndWarning()
        {
            foreach (var file in _project.Files)
            {
                file.ErrorAndWarning = "";
            }
        }
        private int ResetGlobalAffectRulesParameters()
        {
            int countGlobalRule = 0;

            foreach (var rule in _project.RuleList.Rules)
            {
                if (rule.HaveGlobalAffect)
                {
                    rule.ResetGlobalParameter();
                    countGlobalRule++;
                }
                else
                {
                    // Do nothing
                }
            }

            return countGlobalRule;
        }
        private bool ResetGlobalAffectRules()
        {
            bool isResetPerformed = false;

            int countGlobalRule = ResetGlobalAffectRulesParameters();

            if (countGlobalRule > 0)
            {
                ResetErrorAndWarning();
                ResetToOldName();

                ApplyRulesToFileList(
                    startFileIndex: 0,
                    endFileIndex: _project.Files.Count - 1,
                    startRuleIndex: 0,
                    endRuleIndex: _project.RuleList.Rules.Count - 1
                );

                isResetPerformed = true;
            }
            else
            {
                isResetPerformed = false;
            }

            return isResetPerformed;
        }
        #endregion

        #region "BatchRename-Process"
        private void SelectNewLocation_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = folderDialog.SelectedPath;
                NewLocationTextBox.Text = path;
            }
            else
            {
                // Do nothing
            }
        }
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NewLocationCheckBox.IsChecked == true) 
                {
                    string path = NewLocationTextBox.Text;
                    if (Directory.Exists(path))
                    {
                        NewLocation = path;
                        BatchRename(shouldUseNewLocation: true);
                    }
                    else
                    {
                        var shouldUseFolderDialog = System.Windows.MessageBox.Show(
                            string.Concat(
                                $"The directory: \"{path}\" doesn't exist.\n",
                                "Would you like to open a folder dialog to choose?"
                            ),
                            "Wrong Directory",
                            MessageBoxButton.YesNo
                        ) == MessageBoxResult.Yes;

                        if (shouldUseFolderDialog)
                        {
                            CommonOpenFileDialog commonFileDialog = new CommonOpenFileDialog()
                            {
                                IsFolderPicker = true,
                                Multiselect = false,
                                Title = "Select your folder"
                            };

                            if (commonFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                            {
                                NewLocation = commonFileDialog.FileName;
                                NewLocationTextBox.Text = NewLocation;
                            }
                            else
                            {
                                // Do nothing
                            }
                        }
                        else
                        {
                            // Do nothing
                        }   
                    }
                }
                else
                {
                    BatchRename(shouldUseNewLocation: false);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void BatchRename(bool shouldUseNewLocation)
        {
            int nDuplicated = ResolveDuplicatedNewFullPathsBeforeExecute(
                shouldUseNewLocation
            );

            bool success = true;
            string message = "";
            if (nDuplicated > 0)
            {
                var shouldContinue = System.Windows.MessageBox.Show(
                    string.Concat(
                        "Added counter(s) at the end of the new name(s) due to duplication, ",
                        $"{nDuplicated} item(s) will be renamed.\n",
                        "Would you like to continue the execution?"
                    ),
                    "Duplication Warning",
                    MessageBoxButton.YesNo
                ) == MessageBoxResult.Yes;
            
                if (shouldContinue)
                {
                    (success, message) = Rename(shouldUseNewLocation);
                }
                else
                {
                    ResetErrorAndWarning();
                    ResetGlobalAffectRulesParameters();

                    ApplyRulesToFileList(
                        startRuleIndex: 0,
                        endRuleIndex: _project.RuleList.Rules.Count - 1,
                        startFileIndex: 0,
                        endFileIndex: _project.Files.Count - 1
                    );

                    return;
                }
            }
            else
            {
                (success, message) = Rename(shouldUseNewLocation);
            }

            if (success)
            {
                System.Windows.MessageBox.Show(
                    "Successfully Batch Renaming!",
                    "Batch Rename Result"
                );
            }
            else
            {
                System.Windows.MessageBox.Show(
                    message,
                    "Batch Rename Result"
                );
            }
        }
        private Tuple<bool, string> Rename(bool shouldUseNewLocation)
        {
            bool success = true;
            string message = "";

            try
            {
                foreach (var item in _project.Files)
                {
                    if (!item.IsFolder) 
                    {
                        string sourceFileName = "";
                        string destFileName = "";

                        (sourceFileName, destFileName) = 
                            FileItemToSourceAndDestinationPath(
                                item, 
                                shouldUseNewLocation
                            );
                        
                        if (File.Exists(sourceFileName))
                        {
                            if (File.Exists(destFileName))
                            {
                                string duplicationFolder = NewDuplicateFolder;
                                System.IO.Directory.CreateDirectory(duplicationFolder);
                                duplicationFolder = string.Concat(
                                    duplicationFolder,
                                    AppDomain.CurrentDomain.BaseDirectory.Last(),
                                    item.NewName
                                );
                                
                                File.Move(destFileName, duplicationFolder);
                            }
                            else
                            {
                                // Do nothing
                            }

                            try
                            {
                                if (shouldUseNewLocation)
                                {
                                    File.Copy(sourceFileName, destFileName);
                                }
                                else
                                {
                                    File.Move(sourceFileName, destFileName);
                                }

                                item.ErrorAndWarning = string.Concat(
                                    "Successfully Renamed. Ignored processing error or warning: ",
                                    item.ErrorAndWarning
                                );
                            }
                            catch(Exception ex )
                            {
                                item.ErrorAndWarning = string.Concat(
                                    item.ErrorAndWarning,
                                    ex.Message,
                                    "; "
                                );
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(
                                $"\"{sourceFileName}\" -> the path doesn't exist"
                            );
                        }
                    }
                    else
                    {
                        // Do nothing
                    }
                }

                foreach (var item in _project.Files)
                {
                    if (item.IsFolder)
                    {
                        string sourceFileName = "";
                        string destFileName = "";

                        (sourceFileName, destFileName) =
                            FileItemToSourceAndDestinationPath(
                                item,
                                shouldUseNewLocation
                            );

                        if (Directory.Exists(sourceFileName))
                        {
                            if (Directory.Exists(destFileName))
                            {
                                string duplicationFolder = NewDuplicateFolder;
                                System.IO.Directory.CreateDirectory(duplicationFolder);

                                duplicationFolder = string.Concat(
                                    duplicationFolder,
                                    AppDomain.CurrentDomain.BaseDirectory.Last(),
                                    item.NewName
                                );

                                Directory.Move(destFileName, duplicationFolder);
                            }

                            try
                            {
                                if (shouldUseNewLocation)
                                {
                                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(
                                        sourceFileName, destFileName
                                    );
                                }
                                else
                                {
                                    Directory.Move(sourceFileName, destFileName);
                                }

                                item.ErrorAndWarning = string.Concat(
                                    "Successfully Renamed. Ignored processing error or warning: ",
                                    item.ErrorAndWarning
                                );
                            }
                            catch (Exception ex)
                            {
                                item.ErrorAndWarning = string.Format(
                                    item.ErrorAndWarning,
                                    ex.Message,
                                    "Can't move (you may be opening the source folder or there may be some errors)",
                                    "; "
                                );
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(
                                $"\"{sourceFileName}\" -> the path doesn't exist"
                            );
                        }
                    }
                    else
                    {
                        // Do nothing
                    }
                }

                success = true;
            }
            catch(Exception ex)
            {
                success = false;
                message = string.Concat(
                    ex.Message,
                    ": ",
                    ex.StackTrace
                );
            }

            return new Tuple<bool, string>(success, message);
        }
        #endregion

        #region "Save-and-Load-Project"
        private bool ManuallySaveProject()
        {
            bool isDialogCancelled = false;

            var saveDialog = new SaveFileDialog()
            {
                Title = "Save your project",
                Filter = string.Concat(
                        "Batching Ranger Project|*.br-arren|",
                        "Any available file|*.*"
                ),
                FileName = "Untitled Project.br-arren"
            };


            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filepath = saveDialog.FileName;

                bool success = true;
                string message = "";
                (success, message) = ProjectUtils.WriteProject(filepath, _project);

                if (success)
                {
                    System.Windows.MessageBox.Show(
                        "Successfully Saved Your Project",
                        "Save Project Result"
                    );

                    isDialogCancelled = false;
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        message,
                        "Save Project Result"
                    );

                    isDialogCancelled = true;
                }
            }
            else
            {
                isDialogCancelled = true;
            }

            return isDialogCancelled;
        }
        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            ManuallySaveProject();
        }
        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var shouldSaveCurrent = System.Windows.MessageBox.Show(
                    string.Concat(
                        $"Do you want to save the current project before the program opens another one?"
                    ),
                    "Save project",
                    MessageBoxButton.YesNo
                ) == MessageBoxResult.Yes;

                bool isDialogCancelled = false;
                if (shouldSaveCurrent)
                {
                    isDialogCancelled = ManuallySaveProject();
                }
                else
                {
                    // Do nothing
                }    

                if (!isDialogCancelled)
                {
                    CommonOpenFileDialog commonFileDialog = new CommonOpenFileDialog()
                    {
                        Title = "Select your project to open"
                    };

                    commonFileDialog.Filters.Add(
                        new CommonFileDialogFilter("Batching Ranger Project", "*.br-arren")
                    );

                    commonFileDialog.Filters.Add(
                        new CommonFileDialogFilter("Any available file", "*.*")
                    );  

                    if (commonFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        var filepath = commonFileDialog.FileName;

                        bool success = true;
                        string message = "";
                        Project project = new Project()
                        {
                            RuleList = new Preset
                            {
                                Rules = new ObservableCollection<IRule> { }
                            },
                            Files = new ObservableCollection<FileItem> { }
                        };

                        (success, message, project) = ProjectUtils.ReadProject(filepath);

                        if (success) 
                        {
                            _project.RuleList.Rules.Clear();
                            _project.Files.Clear();

                            foreach (var rule in project.RuleList.Rules)
                            {
                                _project.RuleList.Rules.Add(rule);
                            }
                        
                            foreach (var file in project.Files)
                            {
                                _project.Files.Add(file);
                            }

                            System.Windows.MessageBox.Show(
                                "Successfully read your project", 
                                "Read Project Result"
                            );
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(message, "Read Project Result");
                        }
                    }
                    else
                    {
                        // Do nothing
                    }

                } 
                else
                {
                    // Do nothing
                }    
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Read Project Exception");
            }
        }
        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var shouldSaveCurrent = System.Windows.MessageBox.Show(
                    string.Concat(
                        $"Do you want to save the current project before the program opens another one?"
                    ),
                    "Save project",
                    MessageBoxButton.YesNo
                ) == MessageBoxResult.Yes;

                bool isDialogCancelled = false;
                if (shouldSaveCurrent)
                {
                    isDialogCancelled = ManuallySaveProject();
                }
                else
                {
                    // Do nothing
                }    
                
                if (!isDialogCancelled) 
                {
                    _project.RuleList.Rules.Clear();
                    _project.Files.Clear();
                }
                else
                {
                    // Do nothing
                }    
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Read Project Exception");
            }
        }
        #endregion

        #region "Save-and-Load-Preset"
        private void LoadPresetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonOpenFileDialog commonFileDialog = new CommonOpenFileDialog()
                {
                    Title = "Select your preset to open"
                };

                commonFileDialog.Filters.Add(
                    new CommonFileDialogFilter("Batching Ranger Preset", "*.brp-arren")
                );

                commonFileDialog.Filters.Add(
                    new CommonFileDialogFilter("Any available file", "*.*")
                );

                if (commonFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var filepath = commonFileDialog.FileName;

                    bool success = true;
                    string message = "";
                    Preset preset = new Preset()
                    {
                        Rules = new ObservableCollection<IRule> { }
                    };

                    (success, message, preset) = PresetUtils.ReadPreset(filepath);

                    if (success)
                    {
                        _project.RuleList.Rules.Clear();

                        foreach (var rule in preset.Rules)
                        {
                            _project.RuleList.Rules.Add(rule);
                        }

                        ResetErrorAndWarning();
                        ResetToOldName();

                        ApplyRulesToFileList(
                            startRuleIndex: 0,
                            endRuleIndex: _project.RuleList.Rules.Count - 1,
                            startFileIndex: 0,
                            endFileIndex: _project.Files.Count - 1
                        );

                        System.Windows.MessageBox.Show(
                            "Successfully read your preset",
                            "Read Preset Result"
                        );
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(
                            message, 
                            "Read Preset Result"
                        );
                    }
                }
                else
                {
                    // Do nothing
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    ex.Message, 
                    "Read Preset Exception"
                );
            }
        }
        private void SavePresetButton_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog()
            {
                Title = "Save your project",
                Filter = string.Concat(
                        "Batching Ranger Project|*.brp-arren|",
                        "Any available file|*.*"
                ),
                FileName = "Untitled Preset.brp-arren"
            };

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filepath = saveDialog.FileName;

                bool success = true;
                string message = "";
                (success, message) = PresetUtils.WritePreset(
                    filepath, 
                    _project.RuleList
                );

                if (success)
                {
                    System.Windows.MessageBox.Show(
                        "Successfully Saved Your Preset",
                        "Save Preset Result"
                    );
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        message,
                        "Save Preset Result"
                    );
                }
            }
            else
            {
                // Do nothing
            }
        }
        #endregion

        #region "Rule-Setting-Dialog"

        private Dictionary<string, string> _oldRuleParams = new Dictionary<string, string> { };
        private int _affectedRuleIndex = -1;

        private void ShowSettingDialog(int ruleIndex)
        {
            _affectedRuleIndex = ruleIndex;
            
            var ruleToInject = (IRule)_project.RuleList.Rules[ruleIndex].Clone();

            _oldRuleParams = new Dictionary<string, string>(ruleToInject.RuleParameters);

            var ruleSettingUI = new RuleSettingUI(ruleToInject);
            ruleSettingUI.RuleParamValueChangedEvent += RuleSettingUI_ParamValueChanged;

            if (ruleSettingUI.ShowDialog() == true)
            {
                // Do things
            }
            else
            {
                var rule = _project.RuleList.Rules[_affectedRuleIndex];

                if (rule.HaveGlobalAffect)
                {
                    rule.ResetGlobalParameter();
                }
                else
                {
                    // Do nothing
                }

                foreach (var key in _oldRuleParams.Keys)
                {
                    _project
                        .RuleList
                        .Rules[ruleIndex]
                        .RuleParameters[key] 
                    = _oldRuleParams[key];
                }

                ResetToOldName();

                ApplyRulesToFileList(
                    startRuleIndex: 0,
                    endRuleIndex: _project.RuleList.Rules.Count - 1,
                    startFileIndex: 0,
                    endFileIndex: _project.Files.Count - 1
                );
            }

            _affectedRuleIndex = -1;
        }

        private void RuleSettingUI_ParamValueChanged (string key, string value)
        {
            if (_affectedRuleIndex >= 0)
            {
                var rule = _project.RuleList.Rules[_affectedRuleIndex];
                
                if (rule.HaveGlobalAffect)
                {
                    rule.ResetGlobalParameter();
                }
                else
                {
                    // Do nothing
                }

                _project
                    .RuleList
                    .Rules[_affectedRuleIndex]
                    .RuleParameters[key] 
                = value;

                ResetToOldName();

                ApplyRulesToFileList(
                    startRuleIndex: 0,
                    endRuleIndex: _project.RuleList.Rules.Count - 1,
                    startFileIndex: 0,
                    endFileIndex: _project.Files.Count - 1
                );
            }
            else
            {
                // Do nothing
            }
        }

        #endregion
    }
    public static class Extensions
    {
        public static void Swap<T>(this ObservableCollection<T> list, int indexA, int indexB)
        {
            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }
    }
}
