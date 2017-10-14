using System;
using System.Collections.Generic;

namespace WebApiDemo.PlugIn.Models.Api
{
    public interface IPersonService
    {
        IEnumerable<Person> GetAll();

        IEnumerable<Person> GetByOData(object query);

        int CountByOData(object query);

        bool AnyByOData(object query);

        Person GetById(int id, string[] dd);

        IEnumerable<int> GetDataIds();
    }
}