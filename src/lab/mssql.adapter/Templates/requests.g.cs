// <auto-generated>
//     This code was generated  at 2020-07-31 21:08:36.377 
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ProtoBuf;
using collection.extensions;
namespace beatroot_bo
{

#region TypesGetRequest .. 
[DataContract]
public partial class TypesGetRequest
{
   int? _UserStatus;
[DataMember(Order = 1)]
   public int? UserStatus { get { return _GetNullableValue(_UserStatus, 1); } set { _UserStatus = value; _SetNullable(_UserStatus==null,1); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=1;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region TypesGetV2Request .. 
[DataContract]
public partial class TypesGetV2Request
{
   int? _UserStatus;
[DataMember(Order = 1)]
   public int? UserStatus { get { return _GetNullableValue(_UserStatus, 1); } set { _UserStatus = value; _SetNullable(_UserStatus==null,1); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=1;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserAddRequest .. 
[DataContract]
public partial class UserAddRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
   string _Username;
[DataMember(Order = 2)]
   public string Username { get { return _GetNullableValue(_Username, 2); } set { _Username = value; _SetNullable(_Username==null,2); } }
   string _Password;
[DataMember(Order = 3)]
   public string Password { get { return _GetNullableValue(_Password, 3); } set { _Password = value; _SetNullable(_Password==null,3); } }
   string _FullName;
[DataMember(Order = 4)]
   public string FullName { get { return _GetNullableValue(_FullName, 4); } set { _FullName = value; _SetNullable(_FullName==null,4); } }
   string _Email;
[DataMember(Order = 5)]
   public string Email { get { return _GetNullableValue(_Email, 5); } set { _Email = value; _SetNullable(_Email==null,5); } }
   int? _UserIdSupervisor;
[DataMember(Order = 6)]
   public int? UserIdSupervisor { get { return _GetNullableValue(_UserIdSupervisor, 6); } set { _UserIdSupervisor = value; _SetNullable(_UserIdSupervisor==null,6); } }
   byte? _UserStatusTypeId;
[DataMember(Order = 7)]
   public byte? UserStatusTypeId { get { return _GetNullableValue(_UserStatusTypeId, 7); } set { _UserStatusTypeId = value; _SetNullable(_UserStatusTypeId==null,7); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=7;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserRoleTypeAddRequest .. 
[DataContract]
public partial class UserRoleTypeAddRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
   string _Name;
[DataMember(Order = 2)]
   public string Name { get { return _GetNullableValue(_Name, 2); } set { _Name = value; _SetNullable(_Name==null,2); } }
   bool? _IsDisabled;
[DataMember(Order = 3)]
   public bool? IsDisabled { get { return _GetNullableValue(_IsDisabled, 3); } set { _IsDisabled = value; _SetNullable(_IsDisabled==null,3); } }
   string _Description;
[DataMember(Order = 4)]
   public string Description { get { return _GetNullableValue(_Description, 4); } set { _Description = value; _SetNullable(_Description==null,4); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=4;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserRoleTypeGetRequest .. 
[DataContract]
public partial class UserRoleTypeGetRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=1;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserRoleTypeRestrictionAddRequest .. 
[DataContract]
public partial class UserRoleTypeRestrictionAddRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
   byte? _UserRoleTypeId;
[DataMember(Order = 2)]
   public byte? UserRoleTypeId { get { return _GetNullableValue(_UserRoleTypeId, 2); } set { _UserRoleTypeId = value; _SetNullable(_UserRoleTypeId==null,2); } }
   byte? _UserRoleTypeRestrictionTypeId;
[DataMember(Order = 3)]
   public byte? UserRoleTypeRestrictionTypeId { get { return _GetNullableValue(_UserRoleTypeRestrictionTypeId, 3); } set { _UserRoleTypeRestrictionTypeId = value; _SetNullable(_UserRoleTypeRestrictionTypeId==null,3); } }
   string _Value;
[DataMember(Order = 4)]
   public string Value { get { return _GetNullableValue(_Value, 4); } set { _Value = value; _SetNullable(_Value==null,4); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=4;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserRoleTypeRestrictionDeleteRequest .. 
[DataContract]
public partial class UserRoleTypeRestrictionDeleteRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
   int? _Id;
[DataMember(Order = 2)]
   public int? Id { get { return _GetNullableValue(_Id, 2); } set { _Id = value; _SetNullable(_Id==null,2); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=2;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserRoleTypeRestrictionGetRequest .. 
[DataContract]
public partial class UserRoleTypeRestrictionGetRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=1;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserRoleTypeRestrictionUpdateRequest .. 
[DataContract]
public partial class UserRoleTypeRestrictionUpdateRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
   int? _Id;
[DataMember(Order = 2)]
   public int? Id { get { return _GetNullableValue(_Id, 2); } set { _Id = value; _SetNullable(_Id==null,2); } }
   byte? _UserRoleTypeId;
[DataMember(Order = 3)]
   public byte? UserRoleTypeId { get { return _GetNullableValue(_UserRoleTypeId, 3); } set { _UserRoleTypeId = value; _SetNullable(_UserRoleTypeId==null,3); } }
   byte? _UserRoleTypeRestrictionTypeId;
[DataMember(Order = 4)]
   public byte? UserRoleTypeRestrictionTypeId { get { return _GetNullableValue(_UserRoleTypeRestrictionTypeId, 4); } set { _UserRoleTypeRestrictionTypeId = value; _SetNullable(_UserRoleTypeRestrictionTypeId==null,4); } }
   string _Value;
[DataMember(Order = 5)]
   public string Value { get { return _GetNullableValue(_Value, 5); } set { _Value = value; _SetNullable(_Value==null,5); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=5;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserRoleTypeUpdateRequest .. 
[DataContract]
public partial class UserRoleTypeUpdateRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
   byte? _Id;
[DataMember(Order = 2)]
   public byte? Id { get { return _GetNullableValue(_Id, 2); } set { _Id = value; _SetNullable(_Id==null,2); } }
   string _Name;
[DataMember(Order = 3)]
   public string Name { get { return _GetNullableValue(_Name, 3); } set { _Name = value; _SetNullable(_Name==null,3); } }
   bool? _IsDisabled;
[DataMember(Order = 4)]
   public bool? IsDisabled { get { return _GetNullableValue(_IsDisabled, 4); } set { _IsDisabled = value; _SetNullable(_IsDisabled==null,4); } }
   string _Description;
[DataMember(Order = 5)]
   public string Description { get { return _GetNullableValue(_Description, 5); } set { _Description = value; _SetNullable(_Description==null,5); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=5;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserSessionCreateRequest .. 
[DataContract]
public partial class UserSessionCreateRequest
{
   string _Username;
[DataMember(Order = 1)]
   public string Username { get { return _GetNullableValue(_Username, 1); } set { _Username = value; _SetNullable(_Username==null,1); } }
   string _Password;
[DataMember(Order = 2)]
   public string Password { get { return _GetNullableValue(_Password, 2); } set { _Password = value; _SetNullable(_Password==null,2); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=2;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserSessionInvalidateRequest .. 
[DataContract]
public partial class UserSessionInvalidateRequest
{
   string _Token;
[DataMember(Order = 1)]
   public string Token { get { return _GetNullableValue(_Token, 1); } set { _Token = value; _SetNullable(_Token==null,1); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=1;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserSessionVerifyRequest .. 
[DataContract]
public partial class UserSessionVerifyRequest
{
   string _Token;
[DataMember(Order = 1)]
   public string Token { get { return _GetNullableValue(_Token, 1); } set { _Token = value; _SetNullable(_Token==null,1); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=1;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UsersGetRequest .. 
[DataContract]
public partial class UsersGetRequest
{
   string _Token;
[DataMember(Order = 1)]
   public string Token { get { return _GetNullableValue(_Token, 1); } set { _Token = value; _SetNullable(_Token==null,1); } }
   int? _OriginUserId;
[DataMember(Order = 2)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 2); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,2); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=2;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

#region UserUpdateRequest .. 
[DataContract]
public partial class UserUpdateRequest
{
   int? _OriginUserId;
[DataMember(Order = 1)]
   public int? OriginUserId { get { return _GetNullableValue(_OriginUserId, 1); } set { _OriginUserId = value; _SetNullable(_OriginUserId==null,1); } }
   int? _Id;
[DataMember(Order = 2)]
   public int? Id { get { return _GetNullableValue(_Id, 2); } set { _Id = value; _SetNullable(_Id==null,2); } }
   string _Password;
[DataMember(Order = 3)]
   public string Password { get { return _GetNullableValue(_Password, 3); } set { _Password = value; _SetNullable(_Password==null,3); } }
   bool? _ChangePasswordRequired;
[DataMember(Order = 4)]
   public bool? ChangePasswordRequired { get { return _GetNullableValue(_ChangePasswordRequired, 4); } set { _ChangePasswordRequired = value; _SetNullable(_ChangePasswordRequired==null,4); } }
   string _FullName;
[DataMember(Order = 5)]
   public string FullName { get { return _GetNullableValue(_FullName, 5); } set { _FullName = value; _SetNullable(_FullName==null,5); } }
   string _Email;
[DataMember(Order = 6)]
   public string Email { get { return _GetNullableValue(_Email, 6); } set { _Email = value; _SetNullable(_Email==null,6); } }
   int? _UserIdSupervisor;
[DataMember(Order = 7)]
   public int? UserIdSupervisor { get { return _GetNullableValue(_UserIdSupervisor, 7); } set { _UserIdSupervisor = value; _SetNullable(_UserIdSupervisor==null,7); } }
   byte? _UserStatusTypeId;
[DataMember(Order = 8)]
   public byte? UserStatusTypeId { get { return _GetNullableValue(_UserStatusTypeId, 8); } set { _UserStatusTypeId = value; _SetNullable(_UserStatusTypeId==null,8); } }
   DateTime? _LockedUntil;
[DataMember(Order = 9)]
   public DateTime? LockedUntil { get { return _GetNullableValue(_LockedUntil, 9); } set { _LockedUntil = value; _SetNullable(_LockedUntil==null,9); } }
   DateTime? _LastDateModified;
[DataMember(Order = 10)]
   public DateTime? LastDateModified { get { return _GetNullableValue(_LastDateModified, 10); } set { _LastDateModified = value; _SetNullable(_LastDateModified==null,10); } }
  
#region nullable hack...
 
   private BitArray ___Nulables;
   private static int _MaxOrder=10;
   
   [DataMember(Order = 10000)]
   public byte[] __Nullables { get{ return ___Nulables.ToBytes();} set{ if (value != null) { ___Nulables = new BitArray(value); } else { ___Nulables = null; }; } }

   
  private void _SetNullable(bool isNullValue, int order)
  {
    if (___Nulables == null && !isNullValue) return;
    if (order > _MaxOrder) throw new Exception($"_SetNullable{order} is greater than MaxOrder: {_MaxOrder}");
    if (___Nulables == null) ___Nulables = new BitArray(_MaxOrder);
    ___Nulables.Set(order-1, isNullValue);          
      
  }
  private  T _GetNullableValue<T>(T value, int order)
  {
     if (order > _MaxOrder) throw new Exception($"_SetNullable Invalid Generation Order{order} is greater than max order{_MaxOrder} ");
     if (___Nulables == null) return value;
     if (___Nulables.Get(order-1)) return default;
     return value;
  }

#endregion

  
  
}
#endregion

   
     
}