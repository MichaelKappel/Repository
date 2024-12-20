﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryUpdateAsync<T>
    {
        Task<T> UpdateAsync(T key);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey>
    {
        Task<TResult> UpdateAsync(TKey key);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate update);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate1, TUpdate2>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate1 update1, TUpdate2 update2);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate1, TUpdate2, TUpdate3>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate1 update1, TUpdate2 update2, TUpdate3 update3);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate1, TUpdate2, TUpdate3, TUpdate4>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate1 update1, TUpdate2 update2, TUpdate3 update3, TUpdate4 update4);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate1, TUpdate2, TUpdate3, TUpdate4, TUpdate5>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate1 update1, TUpdate2 update2, TUpdate3 update3, TUpdate4 update4, TUpdate5 update5);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate1, TUpdate2, TUpdate3, TUpdate4, TUpdate5, TUpdate6>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate1 update1, TUpdate2 update2, TUpdate3 update3, TUpdate4 update4, TUpdate5 update5, TUpdate6 update6);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate1, TUpdate2, TUpdate3, TUpdate4, TUpdate5, TUpdate6, TUpdate7>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate1 update1, TUpdate2 update2, TUpdate3 update3, TUpdate4 update4, TUpdate5 update5, TUpdate6 update6, TUpdate7 update7);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate1, TUpdate2, TUpdate3, TUpdate4, TUpdate5, TUpdate6, TUpdate7, TUpdate8>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate1 update1, TUpdate2 update2, TUpdate3 update3, TUpdate4 update4, TUpdate5 update5, TUpdate6 update6, TUpdate7 update7, TUpdate8 update8);
    }

    public interface IRepositoryUpdateAsync<TResult, TKey, TUpdate1, TUpdate2, TUpdate3, TUpdate4, TUpdate5, TUpdate6, TUpdate7, TUpdate8, TUpdate9>
    {
        Task<TResult> UpdateAsync(TKey key, TUpdate1 update1, TUpdate2 update2, TUpdate3 update3, TUpdate4 update4, TUpdate5 update5, TUpdate6 update6, TUpdate7 update7, TUpdate8 update8, TUpdate9 update9);
    }

}

