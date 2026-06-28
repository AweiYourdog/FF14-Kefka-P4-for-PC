using UMADOverlay.Models;

namespace UMADOverlay.ViewModels
{
    /// <summary>State for one P4 gimmick row.</summary>
    public class GimmickRowViewModel : ViewModelBase
    {
        readonly GimmickRow _data;
        int _state; // 0=none 1=shin(blue) 2=gi(red)

        public GimmickRowViewModel(GimmickRow data, Lang lang)
        {
            _data = data;
            UpdateTexts(lang);
            CmdShin = new RelayCommand(() => SetState(1));
            CmdGi   = new RelayCommand(() =>
            {
                if (_data.IsToggle && _state == 2) SetState(1);
                else SetState(2);
            });
            CmdReset = new RelayCommand(() => SetState(0));
        }

        public string Label       { get; private set; } = "";
        public string ShinAnswer  { get; private set; } = "";
        public string GiAnswer    { get; private set; } = "";

        string _answer = "";
        public string Answer { get => _answer; private set => Set(ref _answer, value); }

        bool _isShin;
        public bool IsShin { get => _isShin; private set => Set(ref _isShin, value); }

        bool _isGi;
        public bool IsGi { get => _isGi; private set => Set(ref _isGi, value); }

        public RelayCommand CmdShin  { get; }
        public RelayCommand CmdGi    { get; }
        public RelayCommand CmdReset { get; }

        void SetState(int s)
        {
            _state = s;
            IsShin = s == 1;
            IsGi   = s == 2;
            Answer = s switch { 1 => ShinAnswer, 2 => GiAnswer, _ => "" };
        }

        public void UpdateTexts(Lang lang)
        {
            Label      = I18n.Get(lang, _data.LabelKey);
            ShinAnswer = I18n.Get(lang, _data.ShinAnswerKey);
            GiAnswer   = I18n.Get(lang, _data.GiAnswerKey);
            // Refresh displayed answer if already selected
            Answer = _state switch { 1 => ShinAnswer, 2 => GiAnswer, _ => "" };
            OnPropertyChanged(nameof(Label));
            OnPropertyChanged(nameof(ShinAnswer));
            OnPropertyChanged(nameof(GiAnswer));
        }

        public void Reset() => SetState(0);
    }

    public class SectionViewModel(string sectionKey, GimmickRowViewModel[] rows) : ViewModelBase
    {
        string _header = "";
        public string Header { get => _header; set => Set(ref _header, value); }
        public GimmickRowViewModel[] Rows { get; } = rows;

        public void UpdateTexts(Lang lang)
        {
            Header = I18n.Get(lang, sectionKey);
            foreach (var r in Rows) r.UpdateTexts(lang);
        }

        public void Reset() { foreach (var r in Rows) r.Reset(); }
    }
}
