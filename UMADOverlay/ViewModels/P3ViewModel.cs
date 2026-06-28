using System.Windows.Input;
using UMADOverlay.Models;

namespace UMADOverlay.ViewModels
{
    public class P3ButtonViewModel : ViewModelBase
    {
        bool _isActive;
        public string Name { get; }
        public bool IsActive { get => _isActive; set => Set(ref _isActive, value); }
        public P3ButtonViewModel(string name) => Name = name;
    }

    public class P3ViewModel : ViewModelBase
    {
        // ── Mutex groups (A–N) ────────────────────────────────────
        static readonly string[][] MutexGroups =
        [
            ["A","B"], ["C","D"], ["F","G"], ["H","I"], ["J","K"], ["E","L"], ["M","N"]
        ];

        // ── OP / QR groups (separate, with defaults) ──────────────
        static readonly string[] GroupOP = ["O","P"];
        static readonly string[] GroupQR = ["Q","R"];

        readonly Dictionary<string, P3ButtonViewModel> _btns;
        readonly string[] _answers = new string[9]; // index 1..8

        public P3ViewModel()
        {
            string[] names = ["A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R"];
            _btns = names.ToDictionary(n => n, n => new P3ButtonViewModel(n));

            BtnA = _btns["A"]; BtnB = _btns["B"];
            BtnC = _btns["C"]; BtnD = _btns["D"];
            BtnE = _btns["E"];
            BtnF = _btns["F"]; BtnG = _btns["G"];
            BtnH = _btns["H"]; BtnI = _btns["I"];
            BtnJ = _btns["J"]; BtnK = _btns["K"];
            BtnL = _btns["L"];
            BtnM = _btns["M"]; BtnN = _btns["N"];
            BtnO = _btns["O"]; BtnP = _btns["P"];
            BtnQ = _btns["Q"]; BtnR = _btns["R"];

            // O and Q are default-active
            _btns["O"].IsActive = true;
            _btns["Q"].IsActive = true;

            CmdClick = new RelayCommand<string>(OnClick);
            CmdReset = new RelayCommand(ResetAll);

            Recalculate();
        }

        // ── Button properties ─────────────────────────────────────
        public P3ButtonViewModel BtnA { get; }
        public P3ButtonViewModel BtnB { get; }
        public P3ButtonViewModel BtnC { get; }
        public P3ButtonViewModel BtnD { get; }
        public P3ButtonViewModel BtnE { get; }
        public P3ButtonViewModel BtnF { get; }
        public P3ButtonViewModel BtnG { get; }
        public P3ButtonViewModel BtnH { get; }
        public P3ButtonViewModel BtnI { get; }
        public P3ButtonViewModel BtnJ { get; }
        public P3ButtonViewModel BtnK { get; }
        public P3ButtonViewModel BtnL { get; }
        public P3ButtonViewModel BtnM { get; }
        public P3ButtonViewModel BtnN { get; }
        public P3ButtonViewModel BtnO { get; }
        public P3ButtonViewModel BtnP { get; }
        public P3ButtonViewModel BtnQ { get; }
        public P3ButtonViewModel BtnR { get; }

        // ── Answer properties 1–8 ─────────────────────────────────
        public string Ans1 { get => _answers[1]; private set { _answers[1] = value; OnPropertyChanged(); } }
        public string Ans2 { get => _answers[2]; private set { _answers[2] = value; OnPropertyChanged(); } }
        public string Ans3 { get => _answers[3]; private set { _answers[3] = value; OnPropertyChanged(); } }
        public string Ans4 { get => _answers[4]; private set { _answers[4] = value; OnPropertyChanged(); } }
        public string Ans5 { get => _answers[5]; private set { _answers[5] = value; OnPropertyChanged(); } }
        public string Ans6 { get => _answers[6]; private set { _answers[6] = value; OnPropertyChanged(); } }
        public string Ans7 { get => _answers[7]; private set { _answers[7] = value; OnPropertyChanged(); } }
        public string Ans8 { get => _answers[8]; private set { _answers[8] = value; OnPropertyChanged(); } }

        public bool HasAns1 { get => !string.IsNullOrEmpty(Ans1); }
        public bool HasAns2 { get => !string.IsNullOrEmpty(Ans2); }
        public bool HasAns3 { get => !string.IsNullOrEmpty(Ans3); }
        public bool HasAns4 { get => !string.IsNullOrEmpty(Ans4); }
        public bool HasAns5 { get => !string.IsNullOrEmpty(Ans5); }
        public bool HasAns6 { get => !string.IsNullOrEmpty(Ans6); }
        public bool HasAns7 { get => !string.IsNullOrEmpty(Ans7); }
        public bool HasAns8 { get => !string.IsNullOrEmpty(Ans8); }

        public ICommand CmdClick { get; }
        public ICommand CmdReset { get; }

        // ── Click logic ───────────────────────────────────────────
        void OnClick(string name)
        {
            // OP / QR groups: toggle off if already active (but keep O/Q as fallback)
            if (GroupOP.Contains(name) || GroupQR.Contains(name))
            {
                HandleDefaultGroup(name, GroupOP.Contains(name) ? GroupOP : GroupQR);
                Recalculate();
                return;
            }

            // A–N: standard mutex toggle
            var btn = _btns[name];
            if (btn.IsActive) { btn.IsActive = false; Recalculate(); return; }
            foreach (var group in MutexGroups)
            {
                if (!group.Contains(name)) continue;
                foreach (var g in group)
                    if (g != name) _btns[g].IsActive = false;
            }
            btn.IsActive = true;
            Recalculate();
        }

        /// <summary>
        /// For OP/QR groups: clicking the inactive button activates it.
        /// Clicking the already-active button does nothing (always one active).
        /// </summary>
        void HandleDefaultGroup(string name, string[] group)
        {
            // group[0] = default (O or Q), group[1] = non-default (P or R)
            var defaultBtn = group[0];

            if (_btns[name].IsActive)
            {
                // Already active:
                // O/Q (default) → do nothing
                if (name == defaultBtn) return;
                // P/R (non-default) → switch back to O/Q
                _btns[name].IsActive       = false;
                _btns[defaultBtn].IsActive = true;
            }
            else
            {
                // Not active → activate this, deactivate partner
                foreach (var g in group) _btns[g].IsActive = false;
                _btns[name].IsActive = true;
            }
        }

        bool Is(string n) => _btns[n].IsActive;

        Lang _lang = Lang.ZH;
        string T(string key) => I18n.Get(_lang, key);

        // ── Button label properties ───────────────────────────────
        public string BtnCLabel => T("p3-btn-c");
        public string BtnDLabel => T("p3-btn-d");
        public string Num4Label => T("p3-num-4");
        public string Num7Label => T("p3-num-7");

        // ── Section title properties ──────────────────────────────
        public string SecBomb       => T("p3-sec-bomb");
        public string SecEarly      => T("p3-sec-early");
        public string SecLate       => T("p3-sec-late");
        public string SecMagic      => T("p3-sec-magic");
        public string SecGC1        => T("p3-sec-gc1");
        public string SecWaterFire1 => T("p3-sec-waterfire1");
        public string SecGC2        => T("p3-sec-gc2");
        public string SecWaterFire2 => T("p3-sec-waterfire2");
        public string SecThunder    => T("p3-sec-thunder");
        public string SecIce        => T("p3-sec-ice");

        public void UpdateLang(Lang lang)
        {
            _lang = lang;
            // Refresh section titles
            OnPropertyChanged(nameof(BtnCLabel));
            OnPropertyChanged(nameof(BtnDLabel));
            OnPropertyChanged(nameof(Num4Label));
            OnPropertyChanged(nameof(Num7Label));
            OnPropertyChanged(nameof(SecBomb));
            OnPropertyChanged(nameof(SecEarly));
            OnPropertyChanged(nameof(SecLate));
            OnPropertyChanged(nameof(SecMagic));
            OnPropertyChanged(nameof(SecGC1));
            OnPropertyChanged(nameof(SecWaterFire1));
            OnPropertyChanged(nameof(SecGC2));
            OnPropertyChanged(nameof(SecWaterFire2));
            OnPropertyChanged(nameof(SecThunder));
            OnPropertyChanged(nameof(SecIce));
            Recalculate();
        }

        void Recalculate()
        {
            // 1. E/L → 不要動/動起來
            if      (Is("E") && Is("A")) Set1(T("p3-stop"));
            else if (Is("E") && Is("B")) Set1(T("p3-move"));
            else if (Is("L") && Is("J")) Set1(T("p3-stop"));
            else if (Is("L") && Is("K")) Set1(T("p3-move"));
            else                         Set1("");

            // 2. C/D → 雷散開/水散開
            if      (Is("C") && Is("A")) Set2(T("p3-thunder-out"));
            else if (Is("C") && Is("B")) Set2(T("p3-water-out"));
            else if (Is("D") && Is("J")) Set2(T("p3-thunder-out"));
            else if (Is("D") && Is("K")) Set2(T("p3-water-out"));
            else                         Set2("");

            // 3. A/B → 不要看/要看
            if      (Is("A")) Set3(T("p3-lookaway"));
            else if (Is("B")) Set3(T("p3-lookat"));
            else              Set3("");

            // 4. H+F/G or M/N+I
            if      (Is("H") && Is("F")) Set4(T("p3-out"));
            else if (Is("H") && Is("G")) Set4(T("p3-in"));
            else if (Is("M") && Is("I")) Set4(T("p3-out"));
            else if (Is("N") && Is("I")) Set4(T("p3-in"));
            else                         Set4("");

            // 5. D/C + A/B or J/K
            if      (Is("D") && Is("A")) Set5(T("p3-thunder-out"));
            else if (Is("D") && Is("B")) Set5(T("p3-water-out"));
            else if (Is("C") && Is("J")) Set5(T("p3-thunder-out"));
            else if (Is("C") && Is("K")) Set5(T("p3-water-out"));
            else                         Set5("");

            // 6. J/K → 不要看/要看
            if      (Is("J")) Set6(T("p3-lookaway"));
            else if (Is("K")) Set6(T("p3-lookat"));
            else              Set6("");

            // 7. I+F/G or M/N+H
            if      (Is("I") && Is("F")) Set7(T("p3-in"));
            else if (Is("I") && Is("G")) Set7(T("p3-out"));
            else if (Is("M") && Is("H")) Set7(T("p3-in"));
            else if (Is("N") && Is("H")) Set7(T("p3-out"));
            else                         Set7("");

            // 8. O/P × Q/R
            if      (Is("O") && Is("Q")) Set8(T("p3-avoid-both"));
            else if (Is("O") && Is("R")) Set8(T("p3-cone"));
            else if (Is("P") && Is("Q")) Set8(T("p3-line"));
            else if (Is("P") && Is("R")) Set8(T("p3-both"));
            else                         Set8("");
        }

        void Set1(string v) { Ans1 = v; OnPropertyChanged(nameof(HasAns1)); }
        void Set2(string v) { Ans2 = v; OnPropertyChanged(nameof(HasAns2)); }
        void Set3(string v) { Ans3 = v; OnPropertyChanged(nameof(HasAns3)); }
        void Set4(string v) { Ans4 = v; OnPropertyChanged(nameof(HasAns4)); }
        void Set5(string v) { Ans5 = v; OnPropertyChanged(nameof(HasAns5)); }
        void Set6(string v) { Ans6 = v; OnPropertyChanged(nameof(HasAns6)); }
        void Set7(string v) { Ans7 = v; OnPropertyChanged(nameof(HasAns7)); }
        void Set8(string v) { Ans8 = v; OnPropertyChanged(nameof(HasAns8)); }

        public void ResetAll()
        {
            // Reset A–N
            foreach (var name in new[]{"A","B","C","D","E","F","G","H","I","J","K","L","M","N"})
                _btns[name].IsActive = false;
            // O and Q back to default-active
            _btns["O"].IsActive = true; _btns["P"].IsActive = false;
            _btns["Q"].IsActive = true; _btns["R"].IsActive = false;
            Recalculate();
        }
    }
}
