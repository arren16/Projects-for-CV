%% STUDENT
% Name : PHAM Quoc Vuong
% ID   : [HIDDEN]

%% PROBLEM STATEMENT
% Hint: M=61, Beta=4.5513, A_s=52dB
% Choose the Kaiser window to design a digital FIR lowpass filter with:
%   Passband frequency           : w_p = 0.2pi rad/sample
%   Stopband frequency           : w_s = 0.3pi rad/sample
%   Passband ripple              : R_p = 0.25 dB
%   Minimum stopband attenuation : A_s = 50 dB
% Choose an appropriate window function and determine the impulse response
% and provide a plot of the frequency response of the designed filter.

clear; % Clear the variables

%% FUNCTIONS PROVIDED
function hd = ideal_lp(wc,M)
    % Ideal LowPass filter computation
    % ---------------------------------
    % [hd] = ideal_lp(wc,M)
    % hd = (desired) ideal impulse response between 0 to M-1
    % wc = cutoff frequency in radians
    % M = length of the ideal filter
    alpha = (M-1)/2; 
    n = 0:1:(M-1);
    m = n - alpha; 
    fc = wc/pi; 
    hd = fc*sinc(fc*m);
end

%% CODE
% Filter information
wp = 0.2*pi; % Passband frequency
ws = 0.3*pi; % Stopband frequency
As = 50; % Stopband attenuation

% Kaiser Window: Empirical design equations
delta_w = ws - wp; % Transition width
M = ceil(1 + (As - 7.95) / (2.285 * delta_w)) + 1; % Filter length
beta = 0.1102 * (As - 8.7); % For As >= 50
wc = (wp + ws) / 2; % Ideal LPF cutoff frequency

% Init Kaiser window
w_kaiser = kaiser(M, beta)';

% Impulse response of Kaiser window 
h = ideal_lp(wc, M) .* w_kaiser;

% Calculate amplitude response
% H: Frequency response
% w: Angular frequencies
a = [1]; % Denominator coefficient for pure H(z)
[H, w] = freqz(h, a);

% Find actual As
stopband_indices = find(w >= ws); 
As_actual = -max(20*log10(abs(H(stopband_indices))));

% Plotting
figure;
plot(w/pi, 20*log10(abs(H)));
grid on;
% hold on;
title([ ...
    'Kaiser Window LPF, M = ', num2str(M), ...
    ', \beta = ', num2str(beta), ...
    ', A_{s,actual}(red line) = ', num2str(As_actual) ...
]);
line([0 1], [-As_actual -As_actual], 'Color', 'r', 'LineStyle', '--');
xlabel('Frequency (\pi rad/sample)');
ylabel('Magnitude (dB)');

%%
