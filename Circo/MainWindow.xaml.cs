using System;
using System.Collections.Generic;
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

using NAudio;
using NAudio.Wave;
using NAudio.Dsp;

namespace Circo
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum EstadodeJuego { menu, gameplay, gameOver}

        public EstadodeJuego estadoActual = EstadodeJuego.menu;

        
        
        public void iniciar()
        {
            estadoActual = EstadodeJuego.gameplay;
        }
        
        public MainWindow()
        {
            InitializeComponent();

            if(estadoActual == EstadodeJuego.menu)
            {
                pantallaPrincipal.Children.Add(new Menu(iniciar));
                
            
            }

            
        }

        

        
    }
}
