using mssql.collector.types;
using System;
using System.Collections.Generic;

namespace mssql.collector
{
    public class ContractOrderDictionary
    {
        private Dictionary<string, int> Indexs = new Dictionary<string, int>();
        public ContractOrderDictionary(DatabaseMeta meta)
        {
            if (meta == null) return;
            foreach (var proc in meta.Procedures)
            {
                var maxRequestOrder = 1;
                foreach (var requestItem in proc.Request)
                {
                    SetIndex(GetRequestItemKey(proc.SpName, requestItem.Name), requestItem.Order);
                    maxRequestOrder = Math.Max(requestItem.Order, maxRequestOrder);

                    if (requestItem.TVP != null)
                    {
                        var maxTvpOrder = 1;
                       foreach (var tvpItem in requestItem.TVP)
                        {
                            SetIndex(GetRequestTvpKey(proc.SpName, requestItem.Name, tvpItem.Name), tvpItem.Order);
                            maxTvpOrder = Math.Max(tvpItem.Order, maxTvpOrder);
                        }

                        SetIndex(GetRequestTvpMaxKey(proc.SpName, requestItem.Name), maxTvpOrder);
                    }
                }
                //save the max index of request class
                SetIndex(GetRequestMaxKey(proc.SpName), maxRequestOrder);

                var maxResponseOrder = 3;
                foreach (var respItem in proc.Responses)
                {
                    SetIndex(GetResponseItemKey(proc.SpName, respItem.Name), respItem.Order);
                    maxResponseOrder = Math.Max(respItem.Order, maxResponseOrder);

                    var maxParamOrder = 1;
                    foreach (var param in respItem.Params)
                    {
                        SetIndex(GetResponseResultItemKey(proc.SpName, respItem.Name, param.Name), param.Order);
                        maxResponseOrder = Math.Max(param.Order, maxParamOrder);
                    }

                    SetIndex(GetResponseResultMaxKey(proc.SpName, respItem.Name), maxParamOrder);
                }

                SetIndex(GetResponseMaxKey(proc.SpName), maxResponseOrder);
            }
        }

        public int GetOrAddIndex(string maxKey, string key, int minIndex = 1)
        {
            if (!Indexs.TryGetValue(key, out int value))
            {
                value = minIndex;
                if (Indexs.TryGetValue(maxKey, out int maxvalue))
                {
                    value = maxvalue + 1;
                }

                SetIndex(maxKey, value);
                SetIndex(key, value);
            }
            return value;
        }

        private void SetIndex(string key, int value)
        {
            Indexs[key] = value;
        }

        public int GetOrAddRequestIndex(string spName, string itemName)
        {
            return GetOrAddIndex(GetRequestMaxKey(spName), GetRequestItemKey(spName, itemName));
        }

        public int GetOrAddRequestTvpIndex(string spName, string tvpName, string tvpItemName)
        {
            return GetOrAddIndex(GetRequestTvpMaxKey(spName, tvpName), GetRequestTvpKey(spName, tvpName, tvpItemName));
        }

        public int GetOrAddResponseIndex(string spName, string itemName)
        {
            return GetOrAddIndex(GetResponseMaxKey(spName), GetResponseItemKey(spName, itemName), minIndex: 3/* reserve order indexs for StatusCode,StatusMessage*/);
        }

        public int GetOrAddResponseResultSetIndex(string spName, string resultName, string itemName)
        {
            return GetOrAddIndex(GetResponseResultMaxKey(spName, resultName), GetResponseResultItemKey(spName, resultName, itemName));
        }

        #region helpers..

        private static string GetRequestItemKey(string spName, string itemName)
        {
            return $"{spName}::Request:{itemName}";
        }

        private static string GetRequestMaxKey(string spName)
        {
            return $"{spName}::RequestMax";
        }

        private static string GetRequestTvpKey(string spName, string tvpName, string tvpItemName)
        {
            return $"{spName}::Request::tvp::{tvpName}:{tvpItemName}";
        }

        private static string GetRequestTvpMaxKey(string spName, string tvpName)
        {
            return $"{spName}::RequestMax::tvp:{tvpName}";
        }

        private static string GetResponseItemKey(string spName, string itemName)
        {
            return $"{spName}::Response:{itemName}";
        }

        private static string GetResponseMaxKey(string spName)
        {
            return $"{spName}::ResponsetMax";
        }

        private static string GetResponseResultItemKey(string spName, string resultName, string itemName)
        {
            return $"{spName}::Response::Result:{resultName}:{itemName}";
        }

        private static string GetResponseResultMaxKey(string spName, string resultName)
        {
            return $"{spName}::ResponsetMax::Result:{resultName}";
        }

        #endregion helpers..
    }
}