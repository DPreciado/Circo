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
    /// Lógica de interacción para Gameplay.xaml
    /// </summary>
    public partial class Gameplay : UserControl
    {
        MainWindow.EstadodeJuego estadoActual;
        Action callBackIniciar;

        WaveIn waveIn; //Conexion con microfono
        WaveFormat formato; //Fortmato de audio

        Image img = new Image();
        public Gameplay(Action perder)
        {
            InitializeComponent();
            callBackIniciar = perder;

            //Inicializar la conexion
            waveIn = new WaveIn();

            //Establecer el formato
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            formato = waveIn.WaveFormat;

            //Duracion del buffer
            waveIn.BufferMilliseconds = 70;

            //Con que funcion respondemos
            //Cuando se llena el buffer
            waveIn.DataAvailable += WaveIn_DataAvailable;

            waveIn.StartRecording();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesGrabados = e.BytesRecorded;

            int numMuestras = bytesGrabados / 2;

            int exponente = 0;
            int numeroBits = 0;

            do
            {
                exponente++;
                numeroBits = (int)Math.Pow(2, exponente);
            } while (numeroBits < numMuestras);
            exponente -= 1;
            numeroBits = (int)Math.Pow(2, exponente);
            Complex[] muestrasComplejas = new Complex[numeroBits];

            for (int i = 0; i < bytesGrabados; i += 2)
            {
                short muestra =
                    (short)(buffer[i + 1] << 8 | buffer[i]);
                float muestra32bits =
                    (float)muestra / 32768.0f;
                if (i / 2 < numeroBits)
                {
                    muestrasComplejas[i / 2].X = muestra32bits;
                }
            }

            FastFourierTransform.FFT(true, exponente, muestrasComplejas);

            float[] valoresAbsolutos = new float[muestrasComplejas.Length];

            for (int i = 0; i < muestrasComplejas.Length; i++)
            {
                valoresAbsolutos[i] = (float)
                    Math.Sqrt(
                    (muestrasComplejas[i].X * muestrasComplejas[i].X) +
                    (muestrasComplejas[i].Y * muestrasComplejas[i].Y));
            }

            int indiceValorMaximo =
                valoresAbsolutos.ToList().IndexOf(
                valoresAbsolutos.Max());

            float frecuenciaFundamental =
                (float)(indiceValorMaximo * formato.SampleRate) /
                (float)valoresAbsolutos.Length;
            
            //waveIn.StopRecording();

            if(frecuenciaFundamental < 700)
            {
                rotar(rotacionPersonaje.Angle-10);
            }

            lblHertz.Text = frecuenciaFundamental.ToString("N") + " Hz";


        }

        public void rotar(double angulo)
        {
            RotateTransform cambiarAngulo = new RotateTransform(angulo);
            personaje.RenderTransform = cambiarAngulo;
            rotacionPersonaje.Angle = cambiarAngulo.Angle;
        }

        
    }
}
