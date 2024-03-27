using BatchRename_MainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename_UtilClasses
{
    public class Factory<InterfaceType> where InterfaceType : class, ICloneable
    {
        static Dictionary<string, InterfaceType> _prototypes 
            = new Dictionary<string, InterfaceType>();
        public static bool Register(
            string ClassName,
            InterfaceType Instance
        ) {
            bool isSuccessfullyRegistered = true;
            
            if (_prototypes.ContainsKey(ClassName))
            {
                isSuccessfullyRegistered = false;
            }
            else
            {
                _prototypes.Add(ClassName, (InterfaceType)Instance.Clone());
                isSuccessfullyRegistered = true;
            }

            return isSuccessfullyRegistered;
        }

        private static Factory<InterfaceType>? _instance = null;
        public static Factory<InterfaceType> Instance()
        {
            if (_instance == null)
            {
                _instance = new Factory<InterfaceType>();
            }

            return _instance;
        }
        private Factory()
        {
            // Do nothing
        }
        public Tuple<bool, string, InterfaceType?> Produce(string ClassName)
        {
            bool success = true;
            string message = "";
            InterfaceType? result;
            
            try
            {
                if (_prototypes.ContainsKey(ClassName))
                {
                    InterfaceType prototype = _prototypes[ClassName];
                    result = (InterfaceType)prototype.Clone();
                    success = true;
                }
                else
                {
                    success = false;
                    message = $"Internal error: the rule with ClassName=\"{ClassName}\" might not be registered!";
                    result = default(InterfaceType);
                }
            }
            catch(Exception e) 
            {
                success = false;
                message = string.Concat(e.Message, e.StackTrace);
                result = default(InterfaceType);
            }

            return new Tuple<bool, string, InterfaceType?>(
                success, 
                message, 
                result
            );
        }
        public Tuple<bool, string, IRule?> Produce(MinimalRule minimalRule) 
        {
            bool success = true;
            string message = "";
            IRule? result;

            (success, message, result) = Factory<IRule>
                .Instance()
                .Produce(minimalRule.RuleName);

            if (success) 
            {
                result!.RuleParameters = new Dictionary<string, string>(
                    minimalRule.RuleParameters!
                );
            }
            else
            {
                // Do nothing
            }

            return new Tuple<bool, string, IRule?>(
                success, 
                message, 
                result
            );
        }
    }
}
