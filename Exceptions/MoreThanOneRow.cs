namespace SaviaVetAPI.Exceptions
{
    public class MoreThanOneRowException : Exception
    {
        public MoreThanOneRowException() :base("Ha afectado a mas de una fila, revision urgente de base de datos") {}
    }
}