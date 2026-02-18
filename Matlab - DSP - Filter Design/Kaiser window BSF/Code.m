%% STUDENT
% Name : PHAM Quoc Vuong
% ID   : [HIDDEN]

%% PROBLEM STATEMENT
% Hint: beta = 0.1102(A_s - 8.7)
% The frequency response of an ideal bandstop filter is given by:
%   H_d(e^{jw}) = 1 if     0 <= |w| <  pi/3
%   H_d(e^{jw}) = 0 if  pi/3 <= |w| <= 2pi/3 
%   H_d(e^{jw}) = 1 if 2pi/3  < |w| <= pi
% Using a Kaiser window, design a bandstop filter of length 45
% with stopband attenuation of 60 dB.

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
M = 45; % Filter length
As = 60; % Stopband attenuation
wc1 = pi/3; % Low cut point
wc2 = 2*pi/3; % High cut point

% Kaiser Window: Empirical design equations
beta = 0.1102 * (As - 8.7); % For As >= 50

% We have:
% HPF(wc2) = Allpass - LPF(wc2), where delta function represents allpass
% Bandstop = LPF(wc1) + HPF(wc2)
%          = LPF(wc1) + Allpass - LPF(wc2)
%          = Allpass - [LPF(wc2) - LPF(wc1)]
% Ideal allpass filter
alpha = (M-1)/2; % Linear-phase constraint
n = 0:M-1; 
delta = (n == alpha);

% Ideal bandstop filter
h_desired = delta - (ideal_lp(wc2, M) - ideal_lp(wc1, M));

% Init Kaiser window
w_kaiser = kaiser(M, beta)';

% Impulse response of Kaiser window 
h = h_desired .* w_kaiser;

% Calculate amplitude response
% H: Frequency response
% w: Angular frequencies
a = [1]; % Denominator coefficient for pure H(z)
[H, w] = freqz(h, a);

% Find actual As
stopband_indices = find(w >= wc1 & w <= wc2);
As_actual = -min(20*log10(abs(H(stopband_indices))));

% Plotting
figure;
plot(w/pi, 20*log10(abs(H))); grid on;
title([ ...
    'Kaiser Window BSF, M = ', num2str(M), ...
    ', \beta = ', num2str(beta), ...
    ', A_{s,actual}(red line) = ', num2str(As_actual) ...
]);
line([0 1], [-As_actual -As_actual], 'Color', 'r', 'LineStyle', '--');
xlabel('Frequency (\pi rad/sample)'); ylabel('Magnitude (dB)');

%%
