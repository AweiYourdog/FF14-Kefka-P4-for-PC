using UMADOverlay.Models;

namespace UMADOverlay.ViewModels
{
    public class OverlayViewModel : ViewModelBase
    {
        // ── Default sizes per mode ────────────────────────────────
        // Alpha = 舊P3, Beta = 舊P4
        const double ALPHA_DEFAULT_W = 300, ALPHA_DEFAULT_H = 570;
        const double BETA_DEFAULT_W  = 370, BETA_DEFAULT_H  = 640;

        // ── Tab (Alpha=舊P3 is default) ───────────────────────────
        bool _showBeta;  // false = Alpha, true = Beta
        public bool ShowBeta  { get => _showBeta;  set { Set(ref _showBeta, value); OnPropertyChanged(nameof(ShowAlpha)); } }
        public bool ShowAlpha => !_showBeta;

        // Keep P3/P4 aliases so existing XAML bindings still work
        public bool ShowP3 => ShowAlpha;
        public bool ShowP4 => ShowBeta;

        // ── Alpha layout flip ─────────────────────────────────────
        bool _alphaFlipped;
        public bool AlphaFlipped { get => _alphaFlipped; set => Set(ref _alphaFlipped, value); }

        // ── Language ──────────────────────────────────────────────
        Lang _lang = Lang.ZH;
        public Lang CurrentLang { get => _lang; set { Set(ref _lang, value); ApplyLang(); } }
        public bool IsZH => _lang == Lang.ZH;
        public bool IsJP => _lang == Lang.JP;
        public bool IsEN => _lang == Lang.EN;

        // ── Background opacity — 0% = 不透明, 100% = 完全透明 ────
        int _opacityPct = 25;
        public int OpacityPct
        {
            get => _opacityPct;
            set { Set(ref _opacityPct, Math.Clamp(value, 0, 100)); OnPropertyChanged(nameof(BgColor)); }
        }
        public System.Windows.Media.Color BgColor =>
            System.Windows.Media.Color.FromArgb((byte)((100 - _opacityPct) * 255 / 100), 0x12, 0x12, 0x12);

        // ── Minimize ──────────────────────────────────────────────
        bool _isMinimized;
        public bool IsMinimized { get => _isMinimized; set => Set(ref _isMinimized, value); }

        // ── Window size ───────────────────────────────────────────
        System.Windows.Window? _win;
        public void SetWindow(System.Windows.Window w)
        {
            _win = w;
            // Center window on screen on startup
            var screen = System.Windows.SystemParameters.WorkArea;
            _win.Left = (screen.Width  - _win.Width)  / 2 + screen.Left;
            _win.Top  = (screen.Height - _win.Height) / 2 + screen.Top;
        }

        double _windowWidth = ALPHA_DEFAULT_W;
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                var clamped = Math.Clamp(value, 300, 800);
                if (_win != null) _win.Left += _windowWidth - clamped;
                Set(ref _windowWidth, clamped);
            }
        }

        double _windowHeight = ALPHA_DEFAULT_H;
        public double WindowHeight
        {
            get => _windowHeight;
            set => Set(ref _windowHeight, Math.Clamp(value, 400, 1000));
        }

        // ── Size lock ─────────────────────────────────────────────
        bool _sizeUnlocked;
        public bool SizeUnlocked { get => _sizeUnlocked; set => Set(ref _sizeUnlocked, value); }
        public string LockIcon => _sizeUnlocked ? "🔓" : "🔒";

        // ── Settings menu ─────────────────────────────────────────
        bool _menuOpen;
        public bool MenuOpen { get => _menuOpen; set => Set(ref _menuOpen, value); }

        // ── Menu labels (i18n) ────────────────────────────────────
        public string MenuMode      => I18n.Get(_lang, "menu-mode");
        public string MenuLang      => I18n.Get(_lang, "menu-lang");
        public string MenuOpacity   => I18n.Get(_lang, "menu-opacity");
        public string MenuWidth     => I18n.Get(_lang, "menu-width");
        public string MenuHeight    => I18n.Get(_lang, "menu-height");
        public string MenuResetSize => I18n.Get(_lang, "menu-resetsize");
        public string MenuLock      => I18n.Get(_lang, "menu-lock");
        public string MenuFlip      => I18n.Get(_lang, "menu-flip");

        // ── Sub-ViewModels ────────────────────────────────────────
        public SectionViewModel[] P4Sections { get; }  // Beta content
        public P3ViewModel        P3         { get; } = new();  // Alpha content

        // ── Commands ──────────────────────────────────────────────
        public RelayCommand CmdSwitchAlpha   { get; }
        public RelayCommand CmdSwitchBeta    { get; }
        // Keep old names for any remaining XAML references
        public RelayCommand CmdSwitchP3      { get; }
        public RelayCommand CmdSwitchP4      { get; }
        public RelayCommand CmdLangZH        { get; }
        public RelayCommand CmdLangJP        { get; }
        public RelayCommand CmdLangEN        { get; }
        public RelayCommand CmdMinimize     { get; }
        public RelayCommand CmdToggleLock   { get; }
        public RelayCommand CmdToggleMenu    { get; }
        public RelayCommand CmdReset         { get; }
        public RelayCommand CmdResetSize     { get; }
        public RelayCommand CmdClose         { get; }
        public RelayCommand CmdFlipAlpha     { get; }

        public OverlayViewModel()
        {
            // Default: Alpha (舊P3) shown
            _showBeta = false;

            P4Sections = GimmickData.Sections.Select(s =>
            {
                var rows = s.Rows.Select(r => new GimmickRowViewModel(r, _lang)).ToArray();
                var sec  = new SectionViewModel(s.SectionKey, rows);
                sec.UpdateTexts(_lang);
                return sec;
            }).ToArray();

            CmdSwitchAlpha = new RelayCommand(() => { ShowBeta = false; ApplyDefaultSize(); });
            CmdSwitchBeta  = new RelayCommand(() => { ShowBeta = true;  ApplyDefaultSize(); });
            // Aliases
            CmdSwitchP3    = CmdSwitchAlpha;
            CmdSwitchP4    = CmdSwitchBeta;

            CmdLangZH     = new RelayCommand(() => CurrentLang = Lang.ZH);
            CmdLangJP     = new RelayCommand(() => CurrentLang = Lang.JP);
            CmdLangEN     = new RelayCommand(() => CurrentLang = Lang.EN);
            CmdMinimize   = new RelayCommand(() => IsMinimized = !IsMinimized);
            CmdToggleLock = new RelayCommand(() =>
            {
                SizeUnlocked = !SizeUnlocked;
                OnPropertyChanged(nameof(LockIcon));
                if (_win != null)
                    _win.ResizeMode = SizeUnlocked
                        ? System.Windows.ResizeMode.CanResize
                        : System.Windows.ResizeMode.NoResize;
            });
            CmdToggleMenu = new RelayCommand(() => MenuOpen = !MenuOpen);
            CmdReset      = new RelayCommand(DoReset);
            CmdResetSize  = new RelayCommand(ApplyDefaultSize);
            CmdClose      = new RelayCommand(() => System.Windows.Application.Current.Shutdown());
            CmdFlipAlpha  = new RelayCommand(() => AlphaFlipped = !AlphaFlipped);

            ApplyDefaultSize();
        }

        public void SyncSize(double w, double h)
        {
            _windowWidth  = Math.Clamp(w, 300, 800);
            _windowHeight = Math.Clamp(h, 400, 1000);
            OnPropertyChanged(nameof(WindowWidth));
            OnPropertyChanged(nameof(WindowHeight));
        }

        void ApplyDefaultSize()
        {
            WindowWidth  = _showBeta ? BETA_DEFAULT_W  : ALPHA_DEFAULT_W;
            WindowHeight = _showBeta ? BETA_DEFAULT_H  : ALPHA_DEFAULT_H;
        }

        void ApplyLang()
        {
            foreach (var sec in P4Sections) sec.UpdateTexts(_lang);
            P3.UpdateLang(_lang);
            OnPropertyChanged(nameof(IsZH));
            OnPropertyChanged(nameof(IsJP));
            OnPropertyChanged(nameof(IsEN));
            OnPropertyChanged(nameof(MenuMode));
            OnPropertyChanged(nameof(MenuLang));
            OnPropertyChanged(nameof(MenuOpacity));
            OnPropertyChanged(nameof(MenuWidth));
            OnPropertyChanged(nameof(MenuHeight));
            OnPropertyChanged(nameof(MenuResetSize));
            OnPropertyChanged(nameof(MenuLock));
            OnPropertyChanged(nameof(MenuFlip));
        }

        void DoReset()
        {
            if (!_showBeta) P3.ResetAll();
            else foreach (var sec in P4Sections) sec.Reset();
        }
    }
}
