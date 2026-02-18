%% STUDENT
% Name : PHAM Quoc Vuong
% ID   : [HIDDEN]

%% PROBLEM STATEMENT
% (1) Design the following bandpass filter using the Parks-McClellan algorithm:
%   ws1 =  0.2pi, wp1 = 0.35pi, Rp =  1dB
%   wp2 = 0.65pi, ws2 =  0.8pi, As = 60dB
% 
% (2) Design a highpass filter that has the following specifications:
%   ws =  0.6pi, As =  50dB
%   wp = 0.75pi, Rp = 0.5dB
% 
% (3) Design a staircase filter which has 3 bands:
%       0 <= w <= 0.3pi, Hr(w) =   1, delta1 =  0.01
%   0.4pi <= w <= 0.7pi, Hr(w) = 0.5, delta2 = 0.005
%   0.8pi <= w <=    pi, Hr(w) =   0, delta3 = 0.001

clear; % Clear the variables

%% CODE (1): Parks-McClellan Bandpass Filter Design
% (1) Design the following bandpass filter using the Parks-McClellan algorithm:
%   ws1 =  0.2pi, wp1 = 0.35pi, Rp =  1dB
%   wp2 = 0.65pi, ws2 =  0.8pi, As = 60dB

% Filter information
ws1 = 0.2*pi;  % Lower stopband edge (rad/sample)
wp1 = 0.35*pi; % Lower passband edge (rad/sample)
wp2 = 0.65*pi; % Upper passband edge (rad/sample)
ws2 = 0.8*pi;  % Upper stopband edge (rad/sample)
Rp = 1;        % Passband ripple (dB)
As = 60;       % Stopband attenuation (dB)

% Use relative specifications (Lecture 12) to convert dB to delta
% delta1: Passband
% delta2: Stopband
delta1 = (1-10^(-Rp/20)) / (1+10^(-Rp/20));
delta2 = (1 + delta1) * 10^(-As/20);

% Maximum allowable deviation (tolerances) for each band
devs = [delta2, delta1, delta2];

% Desired amplitudes in each band
% 0: Stopband
% 1: Passband
desired_amplitude = [0, 1, 0];

% Frequency band edges
f_edges = [ws1, wp1, wp2, ws2] / pi;

% Estimate filter order N and weights
% N: Estimated filter order
% f0: Normalized frequency band edges
% m0: Amplitude response
% weights: Weights used to adjust the fit in each frequency band
[N, f0, m0, weights] = firpmord(f_edges, desired_amplitude, devs);

% Filter's impulse response
h = firpm(N, f0, m0, weights);

% Calculate frequency response
% H: Frequency response
% w: Angular frequencies
a = [1]; % Denominator coefficient for pure H(z)
[H, w] = freqz(h, a);

% Plotting
figure(1);
plot(w/pi, 20*log10(abs(H)));
grid on;

% Show filter's information
line([0, ws1/pi], [-As, -As], 'Color', 'r', 'LineStyle', '--'); % Lower Stopband
line([wp1/pi, wp2/pi], [-Rp, -Rp], 'Color', 'g', 'LineStyle', '--'); % Passband
line([ws2/pi, 1], [-As, -As], 'Color', 'r', 'LineStyle', '--'); % Upper Stopband

xlabel('Frequency (\pi rad/sample)');
ylabel('Magnitude (dB)');
title(['Parks-McClellan Bandpass Filter, Order N = ', num2str(N)]);
legend('Filter', 'Stopband limit (-60dB)', 'Passband ripple (-1dB)');

clear; % Clear the variables

%% CODE (2): Highpass Filter Design
% (2) Design a highpass filter that has the following specifications:
%   ws =  0.6pi, As =  50dB
%   wp = 0.75pi, Rp = 0.5dB

% Filter information
ws = 0.6*pi;  % Stopband edge (rad/sample)
wp = 0.75*pi; % Passband edge (rad/sample)
As = 50;      % Stopband attenuation (dB)
Rp = 0.5;     % Passband ripple (dB)

% Convert dB specs to delta - similar to CODE (1)
% delta1: Passband
% delta2: Stopband
delta1 = (1-10^(-Rp/20)) / (1+10^(-Rp/20));
delta2 = (1 + delta1) * 10^(-As/20);

% Parameters for firpmord - similar to CODE (1)
f_edges = [ws, wp] / pi; % Frequency band edges
desired_amp = [0, 1];    % 0 for stopband, 1 for passband
devs = [delta2, delta1]; % Tolerances

% Estimate filter order N and weights
% N: Estimated filter order
% f0: Normalized frequency band edges
% m0: Amplitude response
% weights: Weights used to adjust the fit in each frequency band
[N, f0, m0, weights] = firpmord(f_edges, desired_amp, devs);

% Filter's impulse response
h = firpm(N, f0, m0, weights);

% Calculate frequency response
% H: Frequency response
% w: Angular frequencies
a = [1]; % Denominator coefficient for pure H(z)
[H, w] = freqz(h, a);

% Plotting
figure(2);
plot(w/pi, 20*log10(abs(H)));
grid on;

% Show filter's information
line([0, ws/pi], [-As, -As], 'Color', 'r', 'LineStyle', '--'); % Stopband limit
line([wp/pi, 1], [-Rp, -Rp], 'Color', 'g', 'LineStyle', '--'); % Passband limit
line([wp/pi, 1], [Rp, Rp], 'Color', 'g', 'LineStyle', '--'); % Passband limit (upper)

xlabel('Frequency (\pi rad/sample)');
ylabel('Magnitude (dB)');
title(['Parks-McClellan Highpass Filter, Order N = ', num2str(length(h)-1)]);
legend('Magnitude Response', 'Stopband limit', 'Passband ripple');

clear; % Clear the variables

%% CODE (3): Staircase Filter Design
% (3) Design a staircase filter which has 3 bands:
%       0 <= w <= 0.3pi, Hr(w) =   1, delta1 =  0.01
%   0.4pi <= w <= 0.7pi, Hr(w) = 0.5, delta2 = 0.005
%   0.8pi <= w <=    pi, Hr(w) =   0, delta3 = 0.001

% Filter information
% Band 1
wp1 = 0.3*pi;   % Band edge (rad/sample)
m1 = 1.0;       % Desired amplitude
delta1 = 0.01;  % Tolerance
% Band 2
wp2 = 0.4*pi;   % Start edge (rad/sample)
wp3 = 0.7*pi;   % End edge (rad/sample)
m2 = 0.5;       % Desired amplitude
delta2 = 0.005; % Tolerance
% Band 3
ws1 = 0.8*pi;   % Start edge (rad/sample)
m3 = 0.0;       % Desired amplitude
delta3 = 0.001; % Tolerance

% Frequency band edges
f_edges = [wp1, wp2, wp3, ws1] / pi;

% Desired amplitudes in each band
desired_amplitude = [m1, m2, m3];

% Maximum allowable deviation (tolerances) for each band
devs = [delta1, delta2, delta3];

% Estimate filter order N and weights
% N: Estimated filter order
% f0: Normalized frequency band edges
% m0: Amplitude response
% weights: Weights used to adjust the fit in each frequency band
[N, f0, m0, weights] = firpmord(f_edges, desired_amplitude, devs);

% Filter's impulse response
h = firpm(N, f0, m0, weights);

% Calculate frequency response
% H: Frequency response
% w: Angular frequencies
a = [1]; % Denominator coefficient
[H, w] = freqz(h, a, 1024);

% Plotting
figure(3);
plot(w/pi, abs(H), 'b', 'LineWidth', 1);
grid on;

% Show filter's information
% Band 1
line([0, wp1/pi], [m1+delta1, m1+delta1], 'Color', 'r', 'LineStyle', '--');
line([0, wp1/pi], [m1-delta1, m1-delta1], 'Color', 'r', 'LineStyle', '--');
% Band 2
line([wp2/pi, wp3/pi], [m2+delta2, m2+delta2], 'Color', 'g', 'LineStyle', '--');
line([wp2/pi, wp3/pi], [m2-delta2, m2-delta2], 'Color', 'g', 'LineStyle', '--');
% Band 3
line([ws1/pi, 1], [m3+delta3, m3+delta3], 'Color', 'm', 'LineStyle', '--');

xlabel('Frequency (\pi rad/sample)');
ylabel('Magnitude (Absolute)');
title(['Parks-McClellan Staircase Filter, Order N = ', num2str(N)]);
legend('Filter Response', 'Band 1 ripple', '', 'Band 2 ripple', '', 'Band 3 limit');