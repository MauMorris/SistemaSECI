﻿using System;
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
using System.Windows.Shapes;

namespace SistemaSECI
{
    /// <summary>
    /// Lógica de interacción para VentanaEstadisticas.xaml
    /// </summary>
    public partial class VentanaEstadisticas : Window
    {
        public VentanaEstadisticas()
        {
            InitializeComponent();
        }

        private void botonRegresar_VEstadisticasUsuario_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
