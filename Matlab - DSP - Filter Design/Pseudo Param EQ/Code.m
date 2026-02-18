%% PSEUDO PARAMETRIC EQ DEMO (03 Filter Types: LPF, HPF, Bell/Peaking Filter)
% Name : PHAM, Quoc Vuong
% ID   : [HIDDEN]

clear; clc; close all; % Reset environment

%% SIGNAL DEFINITION
fs = 48000; % Sample rate (Hz)
seconds = 0.3; % Signal length in seconds
N  = fs * seconds; % Signal length in samples

% Frequencies from f_start to f_end with step
f_start = 20; f_end = fs/2; step = 50;
frequencies = f_start:step:f_end;

amplitude = 0.5; 

t = (0:N-1) / fs; % Time vector

x = sum(amplitude * sin(2*pi*frequencies'*t)); % Sum the sine waves

%% FILTERS
% Refs for the filter formulas:
% Cookbook formulae for audio equalizer biquad filter coefficients
% URL: https://shepazu.github.io/Audio-EQ-Cookbook/audio-eq-cookbook.html#:~:text=FYI:%20The%20relationship%20between%20bandwidth,a%202%20=%201%20%2D%20%CE%B1%20BP

function y = applyBellFilter(x, fs, band)
    % BELL/PEAKING EQ
    % Intermediate variable
    A  = 10^(band.gain/40);
    w0 = 2*pi*band.fc / fs; % Normalized radian frequency
    BW = band.BW; % Bandwidth (octaves)
    alpha = sin(w0) * sinh(log(2)/2 * BW * w0/sin(w0));
    
    b0 = 1 + alpha*A;
    b1 = -2*cos(w0);
    b2 = 1 - alpha*A;
    
    a0 = 1 + alpha/A;
    a1 = -2*cos(w0);
    a2 = 1 - alpha/A;
    
    % Normalize (divide the nominator and denominator to make a0=1)
    b = [b0/a0 b1/a0 b2/a0];
    a = [1 a1/a0 a2/a0];
    
    y = filter(b, a, x); % Apply filter
end

function y = applyLowPassFilter(x, fs, band)
    % LOW-PASS BIQUAD with slope control
    Nstage = slopeToOrder(band.slope); % 1 stage = 12 dB/oct, 2 stage = 24 dB/oct, etc.
    Q_stage = sqrt(2)^(1/Nstage);
    w0 = 2*pi*band.fc/fs;
    alpha = sin(w0)/(2*Q_stage);
    
    b0 = (1 - cos(w0))/2;
    b1 = 1 - cos(w0);
    b2 = (1 - cos(w0))/2;
    
    a0 = 1 + alpha;
    a1 = -2*cos(w0);
    a2 = 1 - alpha;
    
    b = [b0/a0 b1/a0 b2/a0];
    a = [1 a1/a0 a2/a0];
    
    % Increase the slope with higher-order filters by cascading
    y = x;
    for k = 1:Nstage
        y = filter(b, a, y);
    end
end

function y = applyHighPassFilter(x, fs, band)
    % HIGH-PASS BIQUAD with slope control
    Nstage = slopeToOrder(band.slope);
    Q_stage = sqrt(2)^(1/Nstage);
    w0 = 2*pi*band.fc/fs;
    alpha = sin(w0)/(2*Q_stage);
    
    b0 = (1 + cos(w0))/2;
    b1 = -(1 + cos(w0));
    b2 = (1 + cos(w0))/2;
    
    a0 = 1 + alpha;
    a1 = -2*cos(w0);
    a2 = 1 - alpha;
    
    b = [b0 b1 b2]/a0;
    a = [1 a1/a0 a2/a0];
    
    % Increase the slope with higher-order filters by cascading
    y = x;
    for k = 1:Nstage
        y = filter(b, a, y); 
    end
end

%% HELPERS
function Nstage = slopeToOrder(slope)
    % Convert slope (dB/oct) to number of 2nd-order biquad stages
    % Each biquad stage ~12 dB/oct

    if slope <= 0
        Nstage = 1;  % Minimum 1 stage
    else
        Nstage = ceil(slope / 12);
    end
end

function formatLogAxis(ax)
    % Common frequencies in x-axis
    ticks = [0 8 20 50 100 200 500 1000 2000 5000 10000 20000];
    ticks = ticks(ticks <= ax.XLim(2)); % Keep valid range
    
    ax.XTick = ticks; % Set tick positions
    ax.XTickLabel = string(ticks); % Show full numbers instead of a * 10^b
end

function plotSpectrum(x, y, fs, lowBand, highBand, bellBand)
    % PLOT INPUT & OUTPUT SPECTRUM (LOG SCALE), FILTER INFO, AND EQ CURVE

    % Hanning window to reduce spectral leakage
    N = length(x);
    Nfft = 8192; % Zero padding for smoother spectrum
    w = hann(N)';                              
    xw = x .* w;
    yw = y .* w;
    X = fft(xw, Nfft) / sum(w);
    Y = fft(yw, Nfft) / sum(w);

    % One-sided spectrum & Energy correction
    X = abs(X(1:Nfft/2+1));
    X(2:end-1) = 2*X(2:end-1);
    Y = abs(Y(1:Nfft/2+1));
    Y(2:end-1) = 2*Y(2:end-1);

    % Frequency axis
    f = (0:Nfft/2)*(fs/Nfft);                  

    epsVal = 1e-12; % Prevent log(0)
    Xdb = 20*log10(X + epsVal); % Magnitude (dB)
    Ydb = 20*log10(Y + epsVal); % Magnitude (dB)
    Ydb_rel = 20*log10(Y ./ (X + epsVal)); % Gain relative to input

    % Remove DC for log plot
    fplot = f(2:end);
    Xdb = Xdb(2:end);
    Ydb = Ydb(2:end);
    Ydb_rel = Ydb_rel(2:end);

    figure;

    % SPECTRUM BEFORE & AFTER EQ
    subplot('Position', [0.08 0.60 0.88 0.32]);
    beforeFreq = semilogx(fplot, Xdb, 'b', 'LineWidth', 1.2); hold on;
    afterFreq = semilogx(fplot, Ydb, 'r', 'LineWidth', 1.2); 
    beforeFreq.Color(4) = 1; % alpha = 1 for Before EQ
    afterFreq.Color(4) = 0.4; % alpha = 0.4 for After EQ

    grid on;
    title('Spectrum Before & After EQ');
    xlabel('Frequency (Hz)');
    ylabel('Magnitude (dB)');
    xlim([20 fs/2]);
    formatLogAxis(gca);
    legend({'Before EQ', 'After EQ'});

    % FILTER INFO
    subplot('Position', [0.08 0.48 0.88 0.05]);
    axis off;
    infoText = sprintf([ 
        'LOW PASS/HIGH CUT : fc = %.0f Hz   slope = %d dB/oct\n' ...
        'HIGH PASS/LOW CUT : fc = %.0f Hz   slope = %d dB/oct\n' ...
        'BELL : fc = %.0f Hz   BW = %.2f oct   gain = %+d dB'],  ...
        lowBand.fc,  lowBand.slope, ...
        highBand.fc, highBand.slope, ...
        bellBand.fc, bellBand.BW, bellBand.gain ...
    );
    text(0.5, 0.5, infoText, 'HorizontalAlignment', 'center', ...
         'VerticalAlignment', 'middle', 'FontSize', 11, 'FontWeight', 'bold');
    title('Applied EQ Settings');

    % EQ CURVE (RELATIVE GAIN)
    subplot('Position', [0.08 0.10 0.88 0.32]);
    semilogx(fplot, Ydb_rel, 'magenta', 'LineWidth', 1.2);
    grid on;
    title('EQ Curve (Relative Gain to Input)');
    xlabel('Frequency (Hz)');
    ylabel('Gain (dB)');
    xlim([20 fs/2]);
    formatLogAxis(gca);
end

%% APPLY THE FILTERS

% Band parameters: 
% fc (HZ) - center/cutoff frequency
% slope (dB/octave): 12n, n = 1, 2, 3, ...
% gain (dB)
% BW (octaves) - bandwidth, oct: X_{n+1} to X_{n}, X is a musical note
lowPassBand  = struct('fc', 8000, 'slope', 24);
highPassBand = struct('fc', 200, 'slope', 12);
bellBand = struct('fc', 1000, 'gain', 50, 'BW', 0.05);

% EQ-ing
y = x;
y = applyLowPassFilter(y, fs, lowPassBand); % Apply LPF
y = applyHighPassFilter(y, fs, highPassBand); % Apply HPF
y = applyBellFilter(y, fs, bellBand); % Apply Bell/Peaking Filter

% Plot spectrum Before vs. After EQ
plotSpectrum(x, y, fs, lowPassBand, highPassBand, bellBand);