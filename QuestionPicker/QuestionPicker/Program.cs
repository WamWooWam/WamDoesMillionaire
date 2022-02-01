using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.FileIO;


var category1 = new List<Question>();
var category2 = new List<Question>();
var category3 = new List<Question>();
var category4 = new List<Question>();


var random = new Random();
var file = new QuestionsFile();

var parser = new TextFieldParser("Questions.csv");
parser.TextFieldType = FieldType.Delimited;
parser.SetDelimiters(",");

while (!parser.EndOfData)
{
    var fields = parser.ReadFields();
    if (fields == null) continue;

    if (fields[0] == "Column1" || string.IsNullOrEmpty(fields[0])) continue;

    var question = fields[0];
    var answers = fields[1..5];
    var answersArray = (string[])answers.Clone();
    var difficulty = double.Parse(fields[5]);

    int n = answersArray.Length;
    while (n > 1)
    {
        int k = random.Next(n--);
        string temp = answersArray[n];
        answersArray[n] = answersArray[k];
        answersArray[k] = temp;
    }

    var questionObj = new Question()
    {
        Text = question,
        Choices = answersArray,
        Answer = answers[0],
        Difficulty = difficulty
    };

    file.AllQuestions.Add(questionObj);

    if (questionObj.Difficulty < 2)
    {
        category1.Add(questionObj);
    }
    else if (questionObj.Difficulty <= 4)
    {
        category2.Add(questionObj);
    }
    else if (questionObj.Difficulty < 8)
    {
        category3.Add(questionObj);
    }
    else
    {
        category4.Add(questionObj);
    }
}


//category1.Shuffle(random);
//category2.Shuffle(random);
//category3.Shuffle(random);
//category4.Shuffle(random);

var randomCategory1 = new RandomList<Question>(category1);
var randomCategory2 = new RandomList<Question>(category2);
var randomCategory3 = new RandomList<Question>(category3);
var randomCategory4 = new RandomList<Question>(category4);

for (int i = 0; i < 4; i++)
{
    var list = new List<Question>();
    for (int j = 0; j < 15; j++)
    {
        if (j < 2)
        {
            list.Add(randomCategory1.Next());
        }
        else if (j < 5)
        {
            list.Add(randomCategory2.Next());
        }
        else if (j < 10)
        {
            list.Add(randomCategory3.Next());
        }
        else 
        {
            list.Add(randomCategory4.Next());
        }
    }

    File.WriteAllText($"{i}.json", JsonSerializer.Serialize(list, new JsonSerializerOptions() { WriteIndented = true }));
}

public class QuestionsFile
{
    [JsonPropertyName("allQuestions")]
    public List<Question> AllQuestions { get; set; } = new List<Question>();
}

public class Question
{
    [JsonPropertyName("question")]
    public string Text { get; set; } = "";

    [JsonPropertyName("choices")]
    public string[] Choices { get; set; } = Array.Empty<string>();

    [JsonPropertyName("answer")]
    public string Answer { get; set; } = "";

    [JsonPropertyName("difficulty")]
    public double Difficulty { get; set; }
}

static class RandomExtensions
{
    public static void Shuffle<T>(this IList<T> array, Random rng)
    {
        int n = array.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}

public class RandomList<T>
{
    private T[] _items;
    private int _index;
    private Random _random;

    public RandomList(IList<T> items)
    {
        _random = new Random();
        _items = new T[items.Count];
        Array.Copy(items.ToArray(), _items, items.Count);
        _items.Shuffle(_random);
    }

    public T Next()
    {
        var item = _items[_index];
        if (_index++ >= (_items.Length - 1))
        {
            _index = 0;
            _items.Shuffle(_random);
        }

        return item;
    }
}