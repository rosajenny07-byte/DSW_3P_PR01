
using DSW_3P_PR01.Models;
using DSW_3P_PR01.Repositories;

namespace DSW_3P_PR01.Services
{
    public class MisionService : IMisionService
    {
        private readonly IMisionRepository _repository;

        public MisionService(IMisionRepository repository)
        {
            _repository = repository;
        }

        public List<Mision> ObtenerTodas()
        {
            return _repository.ObtenerTodas();
        }

        public Mision? ObtenerPorId(int id)
        {
            return _repository.ObtenerPorId(id);
        }

        public Mision Crear(Mision mision)
        {
            ValidarMision(mision);

            // Toda misión nueva inicia sin completar.
            mision.Id = 0;
            mision.Completada = false;

            return _repository.Crear(mision);
        }

        public bool Actualizar(int id, Mision mision)
        {
            ValidarMision(mision);

            if (_repository.ObtenerPorId(id) == null)
            {
                return false;
            }

            // El ID de la ruta es el que se conserva.
            mision.Id = id;

            return _repository.Actualizar(mision);
        }

        public bool Eliminar(int id)
        {
            return _repository.Eliminar(id);
        }

        private void ValidarMision(Mision mision)
        {
            if (string.IsNullOrWhiteSpace(mision.Nombre))
            {
                throw new ArgumentException(
                    "El nombre de la misión es obligatorio."
                );
            }

            string[] dificultades =
            {
                "Fácil",
                "Media",
                "Difícil"
            };

            if (!dificultades.Contains(mision.Dificultad))
            {
                throw new ArgumentException(
                    "La dificultad debe ser Fácil, Media o Difícil."
                );
            }

            if (mision.RecompensaXP < 0)
            {
                throw new ArgumentException(
                    "La recompensa XP no puede ser negativa."
                );
            }
        }
    }
}
