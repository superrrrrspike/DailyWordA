using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using MvvmHelpers;

namespace DailyWordA.Library.ViewModels;

public class WordQuizViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;

    public WordQuizViewModel(IWordStorage wordStorage) {
        _wordStorage = wordStorage;
        
        UpdateCommand = new RelayCommand(Update);
        CommitCommand = new RelayCommand(Commit);
        RadioCheckedCommand = new RelayCommand<WordObject>(RadioChecked);
        SelectModeCommand = new RelayCommand<string>(SelectMode);
        
        Update();
    }
    
    private WordObject _correctWord;
    public WordObject CorrectWord {
        get => _correctWord;
        set => SetProperty(ref _correctWord, value);
    }

    public static ObservableRangeCollection<string> QuizModes { get; } 
        = ["英文选义", "中文选词"];
    
    private string _selectedMode = QuizModes[0];
    public string SelectedMode {
        get => _selectedMode;
        private set => SetProperty(ref _selectedMode, value);
    }

    private bool _showMode1 = true;
    public bool ShowMode1 {
        get => _showMode1;
        private set => SetProperty(ref _showMode1, value);
    }
    
    public ICommand SelectModeCommand { get; }

    private void SelectMode(string mode) {
        if (mode == QuizModes[0]) {
            SelectedMode = mode;
            ShowMode1 = true;
            Update();
        }
        else if (mode == QuizModes[1]) {
            SelectedMode = mode;
            ShowMode1 = false;
            Update();
        }
    }

    private string _resultText;
    public string ResultText {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
    }

    public ObservableRangeCollection<WordObject> QuizOptions { get; } = new();
    
    private bool _hasAnswered; //已提交答案
    public bool HasAnswered {
        get => _hasAnswered;
        set => SetProperty(ref _hasAnswered, value);
    }
    
    private bool _hasSelected; //已选择选项
    public bool HasSelected {
        get => _hasSelected;
        set => SetProperty(ref _hasSelected, value);
    }
    
    private WordObject _selectedOption;
    public WordObject SelectedOption {
        get => _selectedOption;
        set => SetProperty(ref _selectedOption, value);
    }
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    public ICommand UpdateCommand { get; }
    private void Update() {
        Task.Run(async () => {
            IsLoading = true;
            HasSelected = false;
            HasAnswered = false;
        
            CorrectWord = await _wordStorage.GetRandomWordAsync();
        
            QuizOptions.Clear();
            var wordList = await _wordStorage.GetWordQuizOptionsAsync(_correctWord);
            QuizOptions.AddRange(wordList);
        
            IsLoading = false;
        });
    }
    
    public ICommand RadioCheckedCommand { get; }
    public void RadioChecked(WordObject selectedWordObject) {
        HasSelected = true;
        SelectedOption = selectedWordObject;
    }
    
    public ICommand CommitCommand { get; }
    public void Commit() {
        if (SelectedOption.Word == CorrectWord.Word) {
            ResultText = "恭喜您回答正确！";
        }
        else {
            ResultText = "很遗憾，回答错误啦~";
        }
        HasAnswered = true;
    }

}