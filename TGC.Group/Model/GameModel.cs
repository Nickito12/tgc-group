using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.GameObjects;
using TGC.Core.Collision;
using TGC.Group.Model.GameObjects.Escenario;
using System.IO;
using System;
using System.Windows.Forms;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m�s ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        // El personaje principal
        public Character Personaje = new Character();
        public Escenario Escenario;
        // Para precargar todos los escenarios
        public Dictionary<string, Escenario> Escenarios = new Dictionary<string, Escenario>();
        public TgcThirdPersonCamera NuevaCamara;
        public float CameraOffsetHeight = 20;
        public float CameraOffsetForward = -75;
      

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = "Grupo The Bandicoots";
            Description = "Crash Bandicoot - Plataformas - 2018";
        }
        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, estructuras de optimizaci�n, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            NuevaCamara = new TgcThirdPersonCamera(new TGCVector3(0,0,0), 20, -75, Input);
            Camara = NuevaCamara;
            Personaje.Init(this);
            Escenario = new Escenario1();
            Escenario.Init(this);
            Escenarios["Escenario1"] = Escenario;
            Escenario = new EscenarioMenu();
            Escenarios["Menu"] = Escenario;
            Escenario = new EscenarioMuerte();
            Escenario.Init(this);
            Escenarios["Muerte"] = Escenario;
            Escenario = new EscenarioVictoria();
            Escenario.Init(this);
            Escenarios["Victoria"] = Escenario;
            Escenario = new EscenarioBullet1();
            Escenario.Init(this);
            Escenarios["Bullet1"] = Escenario;
            Escenario = Escenarios["Menu"];
            Personaje.Init(this);
            Escenario.Init(this);
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la l�gica de computo del modelo, as� como tambi�n verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();
            if(Escenario!=null)
                Escenario.Update();
            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqu� todo el c�digo referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones seg�n nuestra conveniencia.
            //PreRender();
           
            Escenario.Render();
            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            //PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecuci�n del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gr�ficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            Personaje.Dispose();
            foreach(var par in Escenarios)
                par.Value.Dispose();
        }
        public void limpiarTexturas() { ClearTextures(); }
        public void RenderizaAxis() { RenderAxis(); }
        public void RenderizaFPS() { RenderFPS(); }
        public void CambiarEscenario(string name)
        {
            Escenario = Escenarios[name];
            Escenario.Reset();
        }

        public void guardarPartida()
        {
            try
            {
                String PathDirectorio = MediaDir + "PartidasGuardadas\\";
                string[] filePaths = Directory.GetFiles(@PathDirectorio);
                int cantidadArchivos = filePaths.Length;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = PathDirectorio;
                saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    StreamWriter arch = new StreamWriter(saveFileDialog.FileName);
                    arch.WriteLine(Personaje.Position().X + "|" + Personaje.Position().Y + "|" + Personaje.Position().Z);
                    arch.WriteLine(Personaje.vidas);
                    arch.WriteLine(Personaje.checkpointReached);
                    arch.WriteLine(Personaje.checkpointReached2);
                    arch.Close();
                }

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error al guardar Partida.");
            }
            finally
            {
                System.Windows.Forms.MessageBox.Show("Partida guardada correctamente.");
            }
    
        }
        public void LoadPartida()
        {
            String line;
            String PathDirectorio = MediaDir + "PartidasGuardadas\\";

            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = PathDirectorio;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            StreamReader reader = new StreamReader(myStream);
                            line = reader.ReadLine();

                            Personaje.yaJugo = true;
                            string[] posicionGuardada = line.Split('|');
                            float X = Convert.ToSingle(posicionGuardada[0]);
                            float Y = Convert.ToSingle(posicionGuardada[1]);
                            float Z = Convert.ToSingle(posicionGuardada[2]);
                            TGCVector3 posicion = new TGCVector3(X, Y, Z);
                            Personaje.Position(posicion);
                            line = reader.ReadLine();

                            Personaje.Vidas(Convert.ToInt32(line));
                            line = reader.ReadLine();
                            Personaje.checkpointReached = Convert.ToBoolean(line);
                            line = reader.ReadLine();
                            Personaje.checkpointReached2 = Convert.ToBoolean(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }


        }
    }
}