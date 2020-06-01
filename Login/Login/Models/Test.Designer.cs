﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
namespace Login.Models
{
    #region 上下文
    
    /// <summary>
    /// 没有元数据文档可用。
    /// </summary>
    public partial class WeDBEntities1 : ObjectContext
    {
        #region 构造函数
    
        /// <summary>
        /// 请使用应用程序配置文件的“WeDBEntities1”部分中的连接字符串初始化新 WeDBEntities1 对象。
        /// </summary>
        public WeDBEntities1() : base("name=WeDBEntities1", "WeDBEntities1")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// 初始化新的 WeDBEntities1 对象。
        /// </summary>
        public WeDBEntities1(string connectionString) : base(connectionString, "WeDBEntities1")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// 初始化新的 WeDBEntities1 对象。
        /// </summary>
        public WeDBEntities1(EntityConnection connection) : base(connection, "WeDBEntities1")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region 分部方法
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet 属性
    
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        public ObjectSet<User> User
        {
            get
            {
                if ((_User == null))
                {
                    _User = base.CreateObjectSet<User>("User");
                }
                return _User;
            }
        }
        private ObjectSet<User> _User;

        #endregion

        #region AddTo 方法
    
        /// <summary>
        /// 用于向 User EntitySet 添加新对象的方法，已弃用。请考虑改用关联的 ObjectSet&lt;T&gt; 属性的 .Add 方法。
        /// </summary>
        public void AddToUser(User user)
        {
            base.AddObject("User", user);
        }

        #endregion

    }

    #endregion

    #region 实体
    
    /// <summary>
    /// 没有元数据文档可用。
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="WeDBModel1", Name="User")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class User : EntityObject
    {
        #region 工厂方法
    
        /// <summary>
        /// 创建新的 User 对象。
        /// </summary>
        /// <param name="id">id 属性的初始值。</param>
        public static User CreateUser(global::System.Int32 id)
        {
            User user = new User();
            user.id = id;
            return user;
        }

        #endregion

        #region 简单属性
    
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    OnidChanging(value);
                    ReportPropertyChanging("id");
                    _id = StructuralObject.SetValidValue(value, "id");
                    ReportPropertyChanged("id");
                    OnidChanged();
                }
            }
        }
        private global::System.Int32 _id;
        partial void OnidChanging(global::System.Int32 value);
        partial void OnidChanged();
    
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String name
        {
            get
            {
                return _name;
            }
            set
            {
                OnnameChanging(value);
                ReportPropertyChanging("name");
                _name = StructuralObject.SetValidValue(value, true, "name");
                ReportPropertyChanged("name");
                OnnameChanged();
            }
        }
        private global::System.String _name;
        partial void OnnameChanging(global::System.String value);
        partial void OnnameChanged();
    
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String password
        {
            get
            {
                return _password;
            }
            set
            {
                OnpasswordChanging(value);
                ReportPropertyChanging("password");
                _password = StructuralObject.SetValidValue(value, true, "password");
                ReportPropertyChanged("password");
                OnpasswordChanged();
            }
        }
        private global::System.String _password;
        partial void OnpasswordChanging(global::System.String value);
        partial void OnpasswordChanged();

        #endregion

    }

    #endregion

}
