using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Configuration;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MvcBiblioteka
{
    /// <summary>
    /// Klasa z metodami łącząca się z Azure DocumentDB.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class DocumentDBRepository<T>
    {
        /// <summary>
        /// Metoda do wydobywania jednego dokumentu (rekordu).
        /// </summary>
        /// <param name="predicate">Warunek, który ma spełnić rekord.</param>
        /// <returns></returns>
        public static T GetItem(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                        .Where(predicate)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        /// <summary>
        /// Metoda edytująca dokument.
        /// </summary>
        /// <param name="id">Id dokumentu, który nadpisujemy.</param>
        /// <param name="item">Uaktualniony obiekt.</param>
        /// <returns></returns>
        public static async Task<Document> UpdateItemAsync(string id, T item)
        {
            Document doc = GetDocument(id);
            return await Client.ReplaceDocumentAsync(doc.SelfLink, item);
        }

        /// <summary>
        /// Metoda usuwająca dokument.
        /// </summary>
        /// <param name="id">Id dokumentu.</param>
        /// <returns></returns>
        public static async Task<Document> DeleteItemAsync(string id)
        {
            Document doc = GetDocument(id);
            return await Client.DeleteDocumentAsync(doc.SelfLink);
        }

        /// <summary>
        /// Metoda wydobywająca dokument z bazy na podstawie id.
        /// </summary>
        /// <param name="id">Id dokumentu.</param>
        /// <returns></returns>
        private static Document GetDocument(string id)
        {
            return Client.CreateDocumentQuery(Collection.DocumentsLink)
                .Where(d => d.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        } 

        /// <summary>
        /// Metoad tworząca nowy dokument.
        /// </summary>
        /// <param name="item">Obiekt zapisany do dokumentu.</param>
        /// <returns></returns>
        public static async Task<Document> CreateItemAsync(T item)
        {
            return await Client.CreateDocumentAsync(Collection.SelfLink, item);
        }

        /// <summary>
        /// Metoda do wydobywania wielu dokumentów (rekordu).
        /// </summary>
        /// <param name="predicate">Warunek, który ma spełnić rekord.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                .Where(predicate)
                .AsEnumerable();
        } 

        /// <summary>
        /// Wczytuje lub tworzy nową bazę danych.
        /// </summary>
        /// <returns></returns>
        private static Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }

        /// <summary>
        /// Wczytuje lub tworzy kolekcję dokumentów.
        /// </summary>
        /// <param name="databaseLink">Adres bazy danych.</param>
        /// <returns></returns>
        private static DocumentCollection ReadOrCreateCollection(string databaseLink)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                var collectionSpec = new DocumentCollection { Id = CollectionId };
                var requestOptions = new RequestOptions { OfferType = "S1" };

                col = Client.CreateDocumentCollectionAsync(databaseLink, collectionSpec, requestOptions).Result;
            }

            return col;
        }

        //Expose the "database" value from configuration as a property for internal use
        private static string databaseId;
        private static String DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = ConfigurationManager.AppSettings["database"];
                }

                return databaseId;
            }
        }

        //Expose the "collection" value from configuration as a property for internal use
        private static string collectionId;
        private static String CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    collectionId = ConfigurationManager.AppSettings["collection"];
                }

                return collectionId;
            }
        }

        //Use the ReadOrCreateDatabase function to get a reference to the database.
        private static Database database;
        private static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadOrCreateDatabase();
                }

                return database;
            }
        }

        //Use the ReadOrCreateCollection function to get a reference to the collection.
        private static DocumentCollection collection;
        private static DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        //This property establishes a new connection to DocumentDB the first time it is used, 
        //and then reuses this instance for the duration of the application avoiding the
        //overhead of instantiating a new instance of DocumentClient with each request
        private static DocumentClient client;
        private static DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["endpoint"];
                    string authKey = ConfigurationManager.AppSettings["authKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }

                return client;
            }
        }
    }
}