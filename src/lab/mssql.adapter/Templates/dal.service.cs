﻿// <auto-generated>
//     This code was generated  at 2020-07-25 19:15:09.514 
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using  could_not_found_the_result_file;
namespace mssql.adapter
{
  public partial class DalService
  {
    private DalServiceOptions options;

    public DalService(IOptions<DalServiceOptions> secrets)
    {
            options = secrets.Value;
    }
    
   }

}
