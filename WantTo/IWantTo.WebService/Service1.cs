using System.ServiceModel.Web;

namespace IWantTo.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IWantToService
    {
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "data/{id}")]
        public Person GetData(int id)
        {
            // lookup person with the requested id 
            return new Person()
            {
                Id = id,
                Name = "Leo Messi"
            };
        }
    }
}
