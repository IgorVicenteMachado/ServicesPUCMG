namespace msuser.ViewModel
{
    public class ResultVM<T> 
    {
        public ResultVM(T data)
        {
            Data = data;
        }

        public ResultVM(List<string> errors)
        {
            Errors = errors;
        }    
        public ResultVM(string error)
        {
            Errors.Add(error);
        }

        public ResultVM(T data, List<string> errors)
        {
            Data = data;
            Errors = errors;
        }

        public T Data { get; private set; }
        public List<string> Errors { get; private set; } = new();
    }
}
