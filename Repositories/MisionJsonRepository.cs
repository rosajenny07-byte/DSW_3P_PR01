using System.Text.Json;
using DSW_3P_PR01.Models;

namespace DSW_3P_PR01.Repositories
{
    public class MisionJsonRepository : IMisionRepository
    {
        private readonly string _rutaArchivo;

        private readonly JsonSerializerOptions _opciones = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private static readonly object _lock = new();

        public MisionJsonRepository(IWebHostEnvironment environment)
        {
            string carpeta = Path.Combine(
                environment.ContentRootPath,
                "Data"
            );

            Directory.CreateDirectory(carpeta);

            _rutaArchivo = Path.Combine(carpeta, "misiones.json");

            lock (_lock)
            {
                if (!File.Exists(_rutaArchivo) || new FileInfo(_rutaArchivo).Length == 0)
                {
                    File.WriteAllText(_rutaArchivo, "[]");
                }
            }
        }

        private List<Mision> LeerMisiones()
        {
            lock (_lock)
            {
                if (!File.Exists(_rutaArchivo))
                {
                    File.WriteAllText(_rutaArchivo, "[]");
                    return new List<Mision>();
                }

                string json = File.ReadAllText(_rutaArchivo);

                if (string.IsNullOrWhiteSpace(json))
                {
                    File.WriteAllText(_rutaArchivo, "[]");
                    return new List<Mision>();
                }

                try
                {
                    return JsonSerializer.Deserialize<List<Mision>>(json, _opciones)
                           ?? new List<Mision>();
                }
                catch (JsonException)
                {
                    File.WriteAllText(_rutaArchivo, "[]");
                    return new List<Mision>();
                }
            }
        }

        private void GuardarMisiones(List<Mision> misiones)
        {
            lock (_lock)
            {
                string json = JsonSerializer.Serialize(
                    misiones,
                    _opciones
                );

                File.WriteAllText(_rutaArchivo, json);
            }
        }

        public List<Mision> ObtenerTodas()
        {
            return LeerMisiones();
        }

        public Mision? ObtenerPorId(int id)
        {
            return LeerMisiones()
                .FirstOrDefault(m => m.Id == id);
        }

        public Mision Crear(Mision mision)
        {
            lock (_lock)
            {
                List<Mision> misiones = LeerMisiones();

                mision.Id = misiones.Count == 0
                    ? 1
                    : misiones.Max(m => m.Id) + 1;

                misiones.Add(mision);

                GuardarMisiones(misiones);

                return mision;
            }
        }

        public bool Actualizar(Mision mision)
        {
            lock (_lock)
            {
                List<Mision> misiones = LeerMisiones();

                int indice = misiones.FindIndex(
                    m => m.Id == mision.Id
                );

                if (indice == -1)
                {
                    return false;
                }

                misiones[indice] = mision;

                GuardarMisiones(misiones);

                return true;
            }
        }

        public bool Eliminar(int id)
        {
            lock (_lock)
            {
                List<Mision> misiones = LeerMisiones();

                Mision? mision = misiones.FirstOrDefault(
                    m => m.Id == id
                );

                if (mision == null)
                {
                    return false;
                }

                misiones.Remove(mision);

                GuardarMisiones(misiones);

                return true;
            }
        }
    }
}
