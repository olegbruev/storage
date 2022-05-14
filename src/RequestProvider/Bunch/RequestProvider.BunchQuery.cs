﻿using Microsoft.EntityFrameworkCore;
using MtdKey.Storage.DataModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MtdKey.Storage
{
    public partial class RequestProvider : IDisposable
    {

        public async Task<RequestResult<BunchPattern>> BunchQueryAsync(Action<RequestFilter> filter)
        {
            RequestFilter requestFilter = new();
            filter.Invoke(requestFilter);
            return await BunchQueryAsync(requestFilter); 
        }

        public async Task<RequestResult<BunchPattern>> BunchQueryAsync(RequestFilter filter)
        {
            var patternResult = new RequestResult<BunchPattern>(true);            

            try
            {

                filter.Ids.AddRange(filter.BunchIds);

                if (string.IsNullOrEmpty(filter.BunchName) is not true)
                {
                    var schema = await GetScheamaAsync(filter.BunchName);
                    var banchId = schema.DataSet.First().BunchPattern.BunchId;
                    filter.Ids.Add(banchId);
                }

                var query = context.Set<Bunch>()
                    .Where(bunch => bunch.DeletedFlag == FlagSign.False)
                    .FilterBasic(filter);                    

                if (string.IsNullOrEmpty(filter.SearchText) is not true)
                {
                    var text = filter.SearchText.ToUpper();
                    query = query.Where(bunch => bunch.Name.ToUpper().Contains(text));
                }

                var dataSet = await query                    
                    .Select(bunch => new BunchPattern
                    {
                        BunchId = bunch.Id,
                        Name = bunch.Name,
                    })
                    .FilterPages(filter.Page, filter.PageSize)
                    .ToListAsync();

                patternResult.FillDataSet(dataSet);
            }
            catch (Exception exception)
            {
                patternResult.SetResultInfo(false, exception);
#if DEBUG
                throw;
#endif
            }

            return patternResult;
        }
    }
}
