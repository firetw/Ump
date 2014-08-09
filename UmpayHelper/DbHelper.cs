using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Ibatis.DataMapper.Configuration;
using Apache.Ibatis.DataMapper.Configuration.Interpreters.Config.Xml;
using Apache.Ibatis.DataMapper;
using Apache.Ibatis.DataMapper.Session;
using Apache.Ibatis.DataMapper.Session.Transaction;
using System.Data;
using Apache.Ibatis.DataMapper.Model.Statements;
using Apache.Ibatis.DataMapper.MappedStatements;
using Apache.Ibatis.DataMapper.Model;
using Apache.Ibatis.DataMapper.Scope;

namespace Umpay.Hjdl
{
    public class DbHelper
    {
        private static readonly DbHelper _instance = new DbHelper();

        public static DbHelper Instance
        {
            get { return _instance; }
        }

        private IDataMapper dataMapper = null;

        public IDataMapper DataMapper
        {
            get { return dataMapper; }

        }
        private ISessionFactory sessionFactory = null;

        public ISessionFactory SessionFactory
        {
            get { return sessionFactory; }

        }
        private ISessionStore sessionStore = null;

        public ISessionStore SessionStore
        {
            get { return sessionStore; }

        }

        public IModelStore ModeStore{get;private set;}
        public DbHelper()
        {
            Initilize();
        }
        public void Initilize()
        {
            ConfigurationSetting configurationSetting = new ConfigurationSetting();
            string resource = AppDomain.CurrentDomain.BaseDirectory + "/Config/SqlMap.config";
            try
            {
                IConfigurationEngine engine = new DefaultConfigurationEngine(configurationSetting);
                engine.RegisterInterpreter(new XmlConfigurationInterpreter(resource));

                ModeStore=engine.ModelStore;
                IMapperFactory mapperFactory = engine.BuildMapperFactory();
                sessionFactory = engine.ModelStore.SessionFactory;
                dataMapper = ((IDataMapperAccessor)mapperFactory).DataMapper;
                sessionStore = ((IModelStoreAccessor)dataMapper).ModelStore.SessionStore;
            }
            catch (Exception ex)
            {
                Exception e = ex;
                while (e != null)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    e = e.InnerException;
                }
                throw;
            }
        }
    }
}
