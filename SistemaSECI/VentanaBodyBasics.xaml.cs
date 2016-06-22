﻿//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="UNAM">
//     Copyright (c) Posgrado en psicologia.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace SistemaSECI
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.ComponentModel;
    using Microsoft.Kinect;
    using System.Threading;
    using System.Globalization;

    /// Interaction logic for VentanaBodyBasics.xaml
    public partial class VentanaBodyBasics : Window, INotifyPropertyChanged
    {

        private const float InferredZPositionClamp = 0.1f;  /// Constante para los vertices inferidos en el eje -Z
        private DrawingGroup drawingGroup;                  /// DrawingGroup para la salida del render para el cuerpo

        private DrawingImage imageSource;                   /// DrawingImage para mostrar
        public ImageSource ImageSource                      /// Gets bitmap a mostrar
        {
            get { return this.imageSource; }
        }

        private KinectBodyTest KBody;

        private string statusText = String.Empty;           /// Estatus actual del texto a mostrar en la pantalla
        public string StatusText                            /// Gets / Sets el estado actual en un texto en pantalla
        {
            get { return this.statusText; }
            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;
                    // notificacion debida al cambio de texto de status 
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatusText"));
                }
            }
        }

        private string nivelText = String.Empty;           /// Estatus actual del nivel jugado
        public string NivelText                            /// Gets / Sets del nivel actual de juego en pantalla
        {
            get { return this.nivelText; }
            set
            {
                if (this.nivelText != value)
                {
                    this.nivelText = value;
                    // notificacion debida al cambio de texto de status 
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NivelText"));
                }
            }
        }

        private int sesionNivel = 0;           /// Estatus actual del nivel jugado
        public int SesionNivel                            /// Gets / Sets del nivel actual de juego en pantalla
        {
            get { return this.sesionNivel; }
            set
            {
                if (this.sesionNivel != value)
                {
                    this.sesionNivel = value;
                    // notificacion debida al cambio de texto de status 
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SesionNivel"));
                }
            }
        }

        private string monedasText = String.Empty;           /// Estatus actual del texto a mostrar en la pantalla
        public string MonedasText                            /// Gets / Sets el estado actual en un texto en pantalla
        {
            get { return this.monedasText; }
            set
            {
                if (this.monedasText != value)
                {
                    this.monedasText = value;
                    // notificacion debida al cambio de texto de status 
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MonedasText"));
                }
            }
        }

        private string timeText = String.Empty;           /// Estatus actual del texto a mostrar en la pantalla
        public string TimeText                            /// Gets / Sets el estado actual en un texto en pantalla
        {
            get { return this.timeText; }
            set
            {
                if (this.timeText != value)
                {
                    this.timeText = value;
                    // notificacion debida al cambio de texto de status 
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimeText"));
                }
            }
        }

        private string jointTextLeft = String.Empty;           /// Estatus actual del nivel jugado
        public string JointTextLeft                           /// Gets / Sets del nivel actual de juego en pantalla
        {
            get { return this.jointTextLeft; }
            set
            {
                if (this.jointTextLeft != value)
                {
                    this.jointTextLeft = value;
                    // notificacion debida al cambio de texto de status 
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("JointsTextLeft"));
                }
            }
        }

        private string jointTextRight = String.Empty;           /// Estatus actual del nivel jugado
        public string JointTextRight                           /// Gets / Sets del nivel actual de juego en pantalla
        {
            get { return this.jointTextRight; }
            set
            {
                if (this.jointTextRight != value)
                {
                    this.jointTextRight = value;
                    // notificacion debida al cambio de texto de status 
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("JointTextRight"));
                }
            }
        }

        private float posicionZ = 0;                        /// Posicion de profundidad del vertice
        public float PosicionZ
        {
            get { return this.posicionZ; }
            set { this.posicionZ = value; }
        }

        private float  posicionZ2 = 0;                      /// Posicion del vertice 2
        public float PosicionZ2
        {
            get { return this.posicionZ2; }
            set { this.posicionZ2 = value; }
        }

        private int monedas = 0;                            /// Monedas acumuladas en la sesion
        public int Monedas
        {
            get { return this.monedas; }
            set { this.monedas = value; }
        }

        private DateTime time0 = DateTime.Now ;                        // Minutos utilizados en la sesion
        public DateTime Time0
        {
            get { return this.time0; }
            set { this.time0 = value; }
        }

        private DateTime time1 = DateTime.Now;                       // Segundos utilizados en la sesion
        public DateTime Time1
        {
            get { return this.time1; }
            set { this.time1 = value; }
        }

        private int nivel;                                  // Nivel actual de la sesion
        public int Nivel
        {
            get { return this.nivel; }
            set { this.nivel = value; }
        }

        private int ejercicios;                             // Ejercicio actual en la sesion
        public int Ejercicios
        {
            get { return this.ejercicios; }
            set { this.ejercicios = value; }
        }

        private int tiempo;                                 // Tiempo utilizado en la sesion
        public int Tiempo
        {
            get { return this.tiempo; }
            set { this.tiempo = value; }
        }

        string pX, pY, pZ;
        bool banderaRepeticion = true;

        BackgroundWorker Cronometro, SubEjercicios;
        public TimeSpan t1;

        /// Inicializa una nueva instancia de MainWindow class
        public VentanaBodyBasics(int nivel, int ejercicios, int tiempo)
        {
            KBody = new KinectBodyTest();

            Nivel = nivel;
            Ejercicios = ejercicios;
            Tiempo = tiempo*60;

            KBody.IniciaComunicacionKinect();
            KBody.IniciaFiguraHumana();

            KBody.KSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;             // set IsAvailableChanged event notifier

            KBody.KSensor.Open();                                                           // open sensor

            NivelText = "NIVEL " + Nivel;                                             // muestra en pantalla en que nivel estas
            TimeText = Time1.Subtract(Time0).ToString();
            // set the status text
            StatusText = KBody.KSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                                : Properties.Resources.NoSensorStatusText;
            this.drawingGroup = new DrawingGroup();                                             // Crea drawing group para dibujar
            this.imageSource = new DrawingImage(this.drawingGroup);                             // Crea una image source para el image control
            this.DataContext = this;                                                            // usa window object como view model

            Monedas = 0;

            Cronometro = new BackgroundWorker();
            SubEjercicios = new BackgroundWorker();

            Cronometro.WorkerReportsProgress = true;
            Cronometro.DoWork += cronometro_DoWork;
            Cronometro.ProgressChanged += cronometro_ProgressChanged;
            Cronometro.RunWorkerCompleted += cronometro_RunWorkerCompleted;

            SubEjercicios.WorkerReportsProgress = true;
            SubEjercicios.DoWork += subEjercicios_DoWork;
            SubEjercicios.ProgressChanged += subEjercicios_ProgressChanged;
            SubEjercicios.RunWorkerCompleted += subEjercicios_RunWorkerCompleted;
        
            Cronometro.RunWorkerAsync(Tiempo);
            Ejercicios--;
            SesionNivel++;
            this.InitializeComponent();                                                         // inicializa (controls) de la ventana
        }

        /// INotifyPropertyChangedPropertyChanged evento para el control de ventana y cambiar los datos con un binding
        public event PropertyChangedEventHandler PropertyChanged;

        /// Handler del evento en el que el sensor no esta disponible (E.g. paused, closed, unplugged).
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        { // en caso de falla, set the status text
            StatusText = KBody.KSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }

        /// Ejecuta tareas iniciales
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BodyBasics_Loaded(object sender, RoutedEventArgs e)
        {
            if (KBody.BFReader != null)
                KBody.BFReader.FrameArrived += this.Reader_FrameArrived;
        }

        /// Ejecuta tareas terminales
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BodyBasics_Closing(object sender, CancelEventArgs e)
        {
            if (KBody.BFReader != null)
            {
                KBody.BFReader.Dispose();   // BodyFrameReader is IDisposable
                KBody.BFReader = null;
            }
        }

        private void cronometro_DoWork(object sender, DoWorkEventArgs e)
        {
            int max = (int) e.Argument;

            for (int i = 0; i < max+1; i++)
            {
                Time1 = DateTime.Now; //Segundo evento

                t1 = Time1.Subtract(Time0); //Diferencia de tiempo

                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(1000);
                e.Result = i;
            }
        }

        void cronometro_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TimeText = t1.TotalSeconds.ToString();
        }

        void cronometro_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var pruebaTerminada = MessageBox.Show("Ejercicio terminado", "Nivel " + nivel, MessageBoxButton.OKCancel, MessageBoxImage.Hand);
            if (pruebaTerminada.Equals(MessageBoxResult.OK))
            {
                if (Ejercicios != 0)
                {
                    Ejercicios--;
                    SesionNivel++;
                    Time0 = DateTime.Now;
                    Time1 = DateTime.Now;

                    Cronometro.RunWorkerAsync(Tiempo);
                }
            }
        }

        private void subEjercicios_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        void subEjercicios_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        void subEjercicios_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        /// Handler para los datos recibidos de body frame enviados por kinect
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (KBody.Bodies == null)
                        KBody.Bodies = new Body[bodyFrame.BodyCount];
                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(KBody.Bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    // Dibuja un background transparente para inicializar el tamaño del render
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, KBody.DWidth, KBody.DHeight));

                    int penIndex = 0;

                    foreach (Body body in KBody.Bodies)
                    {
                        Pen drawPen = KBody.BColors[penIndex++];
                        if (body.IsTracked)
                        {
                            KBody.DrawClippedEdges(body, dc);
                            //coleccion para almacenar los tipos de vertices y la posicion 3D detecta por kinect
                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                            //coleccion para almacenar el tipo de vertice y la orientacion detectada por kinect
                            IReadOnlyDictionary<JointType, JointOrientation> jointsOrientations = body.JointOrientations;
                            // representa la proyeccion de la imagen 3D en un espacio 2D con una profundidad de espacio de referencia.
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            JointTextLeft = "vertice y posicion\n";
                            JointTextRight = "vertice y posicion\n";

                            foreach (JointType jointType in joints.Keys)
                            {
                                //obtiene la posicion 3D de los vertices detectados por kinect
                                CameraSpacePoint position = joints[jointType].Position;
                                // a veces -Z de un vertice inferido puede mostrarse como negativa
                                // clamp down to 0.1f previene que el coordinatemapper regrese valores (-Infinity, -Infinity)
                                if (position.Z < 0)
                                {
                                    position.Z = InferredZPositionClamp;
                                }

                                //programado para conocer la posicion de los vertices en el espacio y mostrarlos en pantalla con orden de hombro
                                if (joints[jointType].Equals(joints[JointType.ShoulderRight]))
                                    PosicionZ = position.Z;
                                if (joints[jointType].Equals(joints[JointType.ShoulderLeft]))
                                    PosicionZ2 = position.Z;
                                if (position.Z < 1)
                                    dc.DrawRectangle(Brushes.Yellow, null, new Rect(0.0, 0.0, KBody.DWidth, KBody.DHeight));
                                else
                                {
                                    if (PosicionZ + 0.15 < PosicionZ2)
                                    {
                                        if(banderaRepeticion.Equals(true))
                                        {
                                            dc.DrawRectangle(Brushes.Blue, null, new Rect(0.0, 0.0, KBody.DWidth, KBody.DHeight));
                                            Monedas++;
                                            MonedasText = Monedas.ToString();
                                            banderaRepeticion = false;
                                        }
                                    }
                                    else
                                    {
                                        banderaRepeticion = true;
                                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, KBody.DWidth, KBody.DHeight));
                                    }
                                }

                                if (jointType == JointType.Head || jointType == JointType.Neck || jointType == JointType.SpineShoulder
                                        || jointType == JointType.ShoulderRight || jointType == JointType.ElbowRight
                                        || jointType == JointType.WristRight || jointType == JointType.HandRight
                                        || jointType == JointType.HipRight || jointType == JointType.KneeRight
                                        || jointType == JointType.AnkleRight || jointType == JointType.FootRight)
                                {
                                    pX = String.Format("{0:#,0.##}", joints[jointType].Position.X);
                                    pY = String.Format("{0:#,0.##}", joints[jointType].Position.Y);
                                    pZ = String.Format("{0:#,0.##}", joints[jointType].Position.Z);

                                    JointTextRight += jointType + "\n" +
                                                    "X: " + pX + "\n" +
                                                    "Y: " + pY + "\n" +
                                                    "Z: " + pZ + "\n";
                                }
                                else
                                {
                                    pX = String.Format("{0:#,0.##}", joints[jointType].Position.X);
                                    pY = String.Format("{0:#,0.##}", joints[jointType].Position.Y);
                                    pZ = String.Format("{0:#,0.##}", joints[jointType].Position.Z);

                                    JointTextLeft += jointType + "\n" +
                                                    "X: " + pX + "\n" +
                                                    "Y: " + pY + "\n" +
                                                    "Z: " + pZ + "\n";
                                }

                                //Conversion de coordenadas 3D a 2D
                                DepthSpacePoint depthSpacePoint = KBody.CMapper.MapCameraPointToDepthSpace(position);
                                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            }
                            KBody.DrawBody(joints, jointPoints, dc, drawPen);
                            KBody.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            KBody.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                        }
                    }
                    // evita que dibujemos fuera del area de render
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, KBody.DWidth, KBody.DHeight));
                }
            }
        }

        private void BotonSalir_VN_click(object sender, RoutedEventArgs e)
        {
            if (KBody.BFReader != null)
            {
                KBody.BFReader.Dispose();                // BodyFrameReader is IDisposable
                KBody.BFReader = null;
            }
            /*////////  if (this.kinectSensor != null) {   this.kinectSensor.Close(); this.kinectSensor = null;  } /////// */
            VentanaEjemplosKinect v = new VentanaEjemplosKinect();
            v.Show();
            this.Close();
        }
    }
}
