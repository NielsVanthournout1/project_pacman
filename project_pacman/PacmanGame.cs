using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
public enum Direction { Up, Down, Left, Right, None }

public class PacmanGame
{
    private Canvas gameCanvas;
    private Ellipse pacman;
    private Direction pacmanDirection;
    private DispatcherTimer gameTimer;
    private List<Rectangle> dots;
    private List<Rectangle> ghosts;
    private int score;
    private int level;
    private bool isGameOver;
    private Direction currentDirection;
    private bool isMoving = false;
    private Label scoreLabel;
    private Label levelLabel;
    private Rectangle ghost;

    // Constructor voor PacmanGame
    public PacmanGame(Canvas canvas, Label scoreLabel, Label levelLabel)
    {
        gameCanvas = canvas;
        pacmanDirection = Direction.Right;
        this.scoreLabel = scoreLabel;
        this.levelLabel = levelLabel;
        InitializeGame();
    }

    // Initialisatie van het spel
    private void InitializeGame()
    {
        score = 0;
        level = 0;

        // Voeg een spook toe
        ghost = new Rectangle
        {
            Width = 20,
            Height = 20,
            Fill = Brushes.Blue,
            Stroke = Brushes.Black
        };

        SetPosition(ghost, 100, 100); // Beginpositie van het spook

        gameCanvas.Children.Add(ghost);

        ghosts = new List<Rectangle>();

        // Zorgt dat de dots kunnen worden toegevoegd
        dots = new List<Rectangle>();

        // Voegd pacman toe
        pacman = new Ellipse
        {
            Width = 30,
            Height = 30,
            Fill = Brushes.Yellow,
            Stroke = Brushes.Black
        };

        SetPosition(pacman, 50, 50); // Veginpositie van pacman

        gameCanvas.Children.Add(pacman);

        // Set de initiële richting van pacman naar None (zodat hij niet beweegd zonder input)
        pacmanDirection = Direction.None;

        // Zegt dat de game kan straten (je bent er niet aan)
        isGameOver = false;
    }

    // Bewegen van het spook
    private void MoveGhost()
    {
        //  Neem de positie van pacman
        double pacmanLeft = Canvas.GetLeft(pacman);
        double pacmanTop = Canvas.GetTop(pacman);

        // Neem de positie van het spook
        double ghostLeft = Canvas.GetLeft(ghost);
        double ghostTop = Canvas.GetTop(ghost);


        // Bereken de richtingsvector van het spook naar Pacman
        double directionX = pacmanLeft - ghostLeft;
        double directionY = pacmanTop - ghostTop;

        // Normaliseer de richtingsvector
        double length = Math.Sqrt(directionX * directionX + directionY * directionY);
        directionX /= length;
        directionY /= length;

        // Beweeg het spook richting Pacman
        SetPosition(ghost, ghostLeft + directionX * 5, ghostTop + directionY * 5);
    }

    // Update de score
    private void UpdateScore()
    {
        // Update de score weergave
        scoreLabel.Content = score.ToString();
    }

    // Controleer of het spel voorbij is
    private void CheckGameOver()
    {
        if (dots.Count == 0 && ghosts.All(ghost => Canvas.GetLeft(ghost) < 1))
        {
            isGameOver = true;
            NextLevel();
        }
    }

    // Ga naar het volgende level
    private void NextLevel()
    {
        // Initialiseer het volgende level
        level++;
        isGameOver = false;

        // Reset Pacman posities
        Canvas.SetLeft(pacman, 50);
        Canvas.SetTop(pacman, 50);

        // Reset spook positie (bedoelt voor meer spoken)
        foreach (var ghost in ghosts)
        {
            Canvas.SetLeft(ghost, level * 50);
            Canvas.SetTop(ghost, 200);
        }

        // Initialiseer nieuwe stippen voor het volgende level
        InitializeDots();

        // Update de level weergave
        levelLabel.Content = level.ToString();
    }

    // Initialisatie van de stippen
    private void InitializeDots()
    {
        dots = new List<Rectangle>();

        for (int i = 0; i < 5 + level; i++)
        {
            for (int j = 0; j < 5 + level; j++)
            {
                var dot = CreateDot();
                SetPosition(dot, i * 50, j * 50);
                dots.Add(dot);
            }
        }
    }

    // Controleer op botsingen
    private void CheckCollision()
    {
        // Controleer botsingen met dots
        foreach (var dot in dots.ToList())
        {
            // Maakt hitboxes voor pacman en dots
            var dotBounds = new Rect(Canvas.GetLeft(dot), Canvas.GetTop(dot), dot.Width, dot.Height);
            var pacmanBounds = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);

            // Controleer of ze elkaar aanraken
            if (pacmanBounds.IntersectsWith(dotBounds))
            {
                gameCanvas.Children.Remove(dot);
                dots.Remove(dot);
                score += 10;
            }
        }

        // Controleer botsingen met spook
        foreach (var ghost in ghosts)
        {
            // Maak hitboxes voor pacman en het spook
            var ghostBounds = new Rect(Canvas.GetLeft(ghost), Canvas.GetTop(ghost), ghost.Width, ghost.Height);
            var pacmanBounds = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);

            // Controleer of ze elkaar aanraken
            if (pacmanBounds.IntersectsWith(ghostBounds))
            {
                MessageBox.Show("Game Over. You were caught by a ghost!");
                isGameOver = true;
                ResetGame();
                break;
            }
        }
    }

    // Reset de game
    private void ResetGame()
    {
        // Stop de game timer
        gameTimer.Stop();

        // Reset de score en level
        score = 0;
        level = 1;

        // Verwijder alle dots
        foreach (var dot in dots)
        {
            gameCanvas.Children.Remove(dot);
        }
        dots.Clear();

        // Verwijder alle ghosts
        foreach (var ghost in ghosts)
        {
            gameCanvas.Children.Remove(ghost);
        }
        ghosts.Clear();

        // Reset pacman positie
        SetPosition(pacman, 50, 50);

        // Reset labels
        UpdateScore();
        levelLabel.Content = level.ToString();

        // Start het spel opnieuw
        isGameOver = false;
        StartGame();
    }

    // Start het spel
    public void StartGame()
    {
        gameTimer = new DispatcherTimer();
        gameTimer.Tick += GameLoop;
        gameTimer.Interval = TimeSpan.FromMilliseconds(100); //stel in hoe rap pacman gaat
        gameTimer.Start();
    }

    // Game loop
    private void GameLoop(object sender, EventArgs e)
    {
        if (!isGameOver)
        {
            MovePacman(pacmanDirection);
            MoveGhost();
            CheckCollision();
            UpdateScore();
            CheckGameOver();
        }
        else
        {
            gameTimer.Stop();
            Console.WriteLine($"Game Over. Your final score: {score}");
        }
    }

    // Beweeg Pacman
    public void MovePacman(Direction direction)
    {
        // Als de nieuwe richting anders is, update de huidige richting
        if (direction != pacmanDirection)
        {
            pacmanDirection = direction;
        }

        // Beweeg Pacman continu in de huidige richting, verander het getal (niet 0) om pacman meer/minder te bewegen
        switch (pacmanDirection)
        {
            case Direction.Up:
                SetPosition(pacman, Canvas.GetLeft(pacman), Math.Max(Canvas.GetTop(pacman) - 10, 0));
                break;
            case Direction.Down:
                SetPosition(pacman, Canvas.GetLeft(pacman), Math.Min(Canvas.GetTop(pacman) + 10, gameCanvas.ActualHeight - pacman.Height));
                break;
            case Direction.Left:
                SetPosition(pacman, Math.Max(Canvas.GetLeft(pacman) - 10, 0), Canvas.GetTop(pacman));
                break;
            case Direction.Right:
                SetPosition(pacman, Math.Min(Canvas.GetLeft(pacman) + 10, gameCanvas.ActualWidth - pacman.Width), Canvas.GetTop(pacman));
                break;
        }
    }

    // Zet de positie van een UI-element
    private void SetPosition(UIElement element, double left, double top)
    {
        Canvas.SetLeft(element, left);
        Canvas.SetTop(element, top);
    }

    // Creeër een stip
    private Rectangle CreateDot()
    {
        var dot = new Rectangle
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.White,
            Stroke = Brushes.Black
        };

        gameCanvas.Children.Add(dot);
        return dot;
    }
}
