using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EnjuAihara.ViewModels.Datatable;

namespace EnjuAihara.Utilities.Datatable
{
    public class PaggingServerSideDatatable
    {
        public static List<T> DatatableSearch<T>(DatatableViewModel model, out int filteredResultsCount, out int totalResultsCount, IQueryable<T> query, string sortByDefault, string strSortDir = null)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                var columnIndex = model.order[0].column;
                //default sort on the 1st column
                sortBy = model.columns[columnIndex].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
                if (sortByDefault != "STT" && sortBy == "STT")
                {
                    sortBy = "";
                    sortDir = (strSortDir == "asc" || strSortDir == null);
                }
            }

            var result = GetData<T>(query, searchBy, take, skip, sortBy, sortByDefault, sortDir, out filteredResultsCount, out totalResultsCount);
            if (result == null)
            {
                return new List<T>();
            }
            return result;
        }
        public static List<T> GetData<T>(IQueryable<T> list, string searchBy, int take, int skip, string sortBy, string sortByDefault, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        {
            if (String.IsNullOrEmpty(sortBy))
            {
                sortBy = sortByDefault;
                //sortDir = true;
            }

            var param = Expression.Parameter(typeof(T), "c");
            var body = sortBy.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            IOrderedQueryable<T> result;
            if (sortBy != "STT")
            {
                result = sortDir == true ?
                (IOrderedQueryable<T>)Queryable.OrderBy(list.AsQueryable(), (dynamic)Expression.Lambda(body, param)) :
                (IOrderedQueryable<T>)Queryable.OrderByDescending(list.AsQueryable(), (dynamic)Expression.Lambda(body, param));
            }
            else
            {
                result = (IOrderedQueryable<T>)list;
            }
            List<T> res = result.Skip(skip).Take(take).ToList();

 
            filteredResultsCount = result.Count();
            totalResultsCount = res.Count();

            return res;
        }

    }
}
