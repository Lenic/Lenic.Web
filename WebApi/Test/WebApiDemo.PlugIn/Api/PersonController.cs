using Lenic.Web.WebApi.Client;
using Lenic.Web.WebApi.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using WebApiDemo.PlugIn.Models;
using WebApiDemo.PlugIn.Models.Api;

namespace WebApiDemo.PlugIn.Api
{
    /// <summary>
    /// 人员信息管理
    /// </summary>
    public class PersonController : ApiController, IPersonService
    {
        public IEnumerable<int> GetDataIds()
        {
            return new int[] { 1, 2, 4 };
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Person> GetAll()
        {
            yield return new Person { Id = 1 };
            yield return new Person
            {
                Id = 2,
                Sex = Sex.Man,
                Name = "张三",
                Address = new Addresses { City = "北京市" },
            };
            yield return new Person { Id = 3 };
        }

        /// <summary>
        /// 根据编号获取人员信息：未找到时返回 <c>null</c> 。
        /// </summary>
        /// <param name="id">待查找人员的编号。</param>
        /// <returns>获取得到的人员信息：未找到时返回 <c>null</c>，请参看 <paramref name="id" />。</returns>
        //[HttpPost]
        public Person GetById(int id, string[] dd)
        {
            return GetAll().FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// 根据指定条件获取符合条件的人员列表。
        /// </summary>
        /// <param name="data">带查询的人员编号的列表。</param>
        /// <returns>符合条件的人员列表。</returns>
        public IList<Person> GetRange(IList<int?> data)
        {
            return null;
        }

        #region IPersonService 成员

        public IEnumerable<Person> GetByOData([ODataBinding]object query)
        {
            var packager = query as ODataQueryPackager;

            return packager.ApplyTo(GetAll().AsQueryable()).OfType<Person>().ToArray();
        }

        [HttpGet]
        public Person[] TestByOData()
        {
            var expr = QueryContext.CreateQuery<Person>().Where(p => p.Id == 2).Query();

            return ODataConverter.CreateInstance<PersonController>(expr, MethodBase.GetCurrentMethod()).ApplyTo(GetAll().AsQueryable()).OfType<Person>().ToArray();
        }

        public int CountByOData([ODataBinding]object query)
        {
            var packager = query as ODataQueryPackager;

            return packager.ApplyTo(GetAll().AsQueryable()).OfType<Person>().Count();
        }

        public bool AnyByOData([ODataBinding]object query)
        {
            var packager = query as ODataQueryPackager;

            return packager.ApplyTo(GetAll().AsQueryable()).OfType<Person>().Any();
        }

        #endregion IPersonService 成员

        public IEnumerable<Person> GetBigData()
        {
            for (int i = 0; i < 100000; i++)
            {
                yield return new Person
                {
                    Id = i,
                    Name = DateTime.Now.ToString(),
                };
            }
        }
    }
}