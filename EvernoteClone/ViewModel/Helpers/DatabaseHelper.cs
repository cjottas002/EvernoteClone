using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EvernoteClone.Model;
using Newtonsoft.Json;
using SQLite;

namespace EvernoteClone.ViewModel.Helpers;

public abstract class DatabaseHelper
{
    private static readonly string DbFile = Path.Combine(Environment.CurrentDirectory, "noteDb.db3");
    private static readonly string DbPath = "https://notes-app-wpf.firebaseio.com/";

    public static async Task<bool> Insert<T>(T item)
    {
        // var result = false;
        //
        // using var conn = new SQLiteConnection(DbFile);
        // conn.CreateTable<T>();
        // var rows = conn.Insert(item);
        // if (rows > 0)
        //     result = true;
        //
        // return result;

        var jsonBody = JsonConvert.SerializeObject(item);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PostAsync($"{DbPath}{item.GetType().Name.ToLower()}.json", content);
        return response.IsSuccessStatusCode;
    }

    public static async Task<bool> Update<T>(T item) where T : IHasId
    {
        // var result = false;
        //
        // using var conn = new SQLiteConnection(DbFile);
        // conn.CreateTable<T>();
        // var rows = conn.Update(item);
        // if (rows > 0)
        //     result = true;
        //
        // return result;
        
        var jsonBody = JsonConvert.SerializeObject(item);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PatchAsync($"{DbPath}{item.GetType().Name.ToLower()}/{item.Id}.json", content);
        return response.IsSuccessStatusCode;
    }

    public static async Task<bool> Delete<T>(T item) where T : IHasId
    {
        // var result = false;
        //
        // using var conn = new SQLiteConnection(DbFile);
        // conn.CreateTable<T>();
        // var rows = conn.Delete(item);
        // if (rows > 0)
        //     result = true;
        //
        // return result;
        
        var jsonBody = JsonConvert.SerializeObject(item);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.DeleteAsync($"{DbPath}{item.GetType().Name.ToLower()}/{item.Id}.json");
        return response.IsSuccessStatusCode;
    }

    public static async Task<List<T>> Read<T>() where T : IHasId
    {
        // List<T> items;
        //
        // using var conn = new SQLiteConnection(DbFile);
        // conn.CreateTable<T>();
        // items = conn.Table<T>().ToList();
        // return items;

        using var client = new HttpClient();
        var result = await client.GetAsync($"{DbPath}{typeof(T).Name.ToLower()}.json");
        var jsonResult = await result.Content.ReadAsStringAsync();

        if (!result.IsSuccessStatusCode) return null;
        var objects = JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonResult);

        var list = new List<T>();
        foreach (var o in objects.Where(o => o.Value != null))
        {
            o.Value.Id = o.Key;
            list.Add(o.Value);
        }

        return list;
    }
}