using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    [SerializeField] TextMeshProUGUI questionText;
    QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    int correctAnswerIndex;
    bool hasAnswerdEarly = true;

    [Header("Button Colors")]
    [SerializeField ]Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;
    [SerializeField] Sprite wrongAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("ProgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;
    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    private void Update()
    {
        timerImage.fillAmount = timer.fillFraction;

        if(timer.loadNextQuestion)
        {
            if (progressBar.value == progressBar.maxValue)
            {
                isComplete = true;
                return;
            }
            hasAnswerdEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if(!hasAnswerdEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1); //tuka se stava -1 za da ne se poklopi slucajno so nekoj od nasite indexi 0,1,2 ili  3
            SetButtonState(false);
        }
    }
    public void OnAnswerSelected(int index)
    {
        hasAnswerdEarly = true;
        DisplayAnswer(index);

        SetButtonState(false);
        timer.CancelTimer();

        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";

        
    }

    void DisplayAnswer(int index)
    {
        Image correctButtonImage;
        Image wrongButtonImage;

        if (index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct!";

            correctButtonImage = answerButtons[index].GetComponent<Image>();
            correctButtonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();
        }
        else
        {
            correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
            questionText.text = "Incorrect. The right answer is: \n" + correctAnswer;

            correctButtonImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            correctButtonImage.sprite = correctAnswerSprite;

            wrongButtonImage = answerButtons[index].GetComponent<Image>();
            wrongButtonImage.sprite = wrongAnswerSprite;

            
        }
    }

    void GetNextQuestion()
    {
        if(questions.Count != 0)
        {
            SetButtonState(true);
            SetDefaultButtonSprites();
            GetRandomQuestion();
            DisplayQuestion();
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
        }
        
    }

    void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];

        if(questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
        
    }
    void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentQuestion.GetAnswer(i);
        }
    }

    void SetButtonState(bool state)
    {
        for(int i = 0; i < answerButtons.Length; i++)
        {
            Button button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    void SetDefaultButtonSprites()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
            
    }

}
