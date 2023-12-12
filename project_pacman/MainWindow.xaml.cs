using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO.Ports;
using System.Reflection;

//microcontroller werkt niet
namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PacmanGame pacmanGame;
        private SerialPort serialPort;

        public MainWindow()
        {
            InitializeComponent();
            // Initialisatie van het PacmanGame-object en de SerialPort
            pacmanGame = new PacmanGame(gameCanvas, scoreLabel, levelLabel);
            serialPort = new SerialPort("COM6", 9600);
            serialPort.DataReceived += SerialPort_DataReceived;

            try
            {
                // Probeer de seriële poort te openen
                serialPort.Open();
            }
            catch (Exception ex)
            {
                // Toon een foutmelding als de poort niet kan worden geopend
                MessageBox.Show("Fout bij het openen van de seriële poort: " + ex.Message);
            }
        }

        // Event handler voor het ontvangen van gegevens op de seriële poort
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Lees de ontvangen gegevens alleen als de poort open is
            if (serialPort.IsOpen)
            {
                string receivedData = serialPort.ReadExisting();
                // Verwerk de ontvangen data
                ProcessReceivedData(receivedData);
            }
        }

        // Methode om ontvangen gegevens te verwerken
        private void ProcessReceivedData(string data)
        {
            // Bepaal de bewegingsrichting op basis van ontvangen data
            if (data.Trim().ToLower() == "button1\r\n")
            {
                Dispatcher.Invoke(() => pacmanGame.MovePacman(Direction.Left));
            }
            else if (data.Trim().ToLower() == "button2\r\n")
            {
                Dispatcher.Invoke(() => pacmanGame.MovePacman(Direction.Right));
            }
            else if (data.Trim().ToLower() == "button3\r\n")
            {
                Dispatcher.Invoke(() => pacmanGame.MovePacman(Direction.Up));
            }
            else if (data.Trim().ToLower() == "button4\r\n")
            {
                Dispatcher.Invoke(() => pacmanGame.MovePacman(Direction.Down));
            }
        }

        // Event handler voor de "Start Game" knop
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Start het spel wanneer op de knop wordt geklikt
            pacmanGame.StartGame();
        }

        // Event handlers voor de bewegingsknoppen
        private void Left_Click(object sender, RoutedEventArgs e)
        {
            pacmanGame.MovePacman(Direction.Left);
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            pacmanGame.MovePacman(Direction.Right);
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            pacmanGame.MovePacman(Direction.Up);
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            pacmanGame.MovePacman(Direction.Down);
        }

        // Event handler voor het sluiten van het hoofdvenster
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Sluit de seriële poort wanneer de applicatie wordt gesloten
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}
