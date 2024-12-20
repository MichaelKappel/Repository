﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryAsync<T, TKey> : IRepositoryCreateAsync<T>, IRepositoryReadAsync<T, TKey>, IRepositoryUpdateAsync<T>, IRepositoryDeleteAsync<T, TKey>, IRepositoryListAsync<T>
    {

    }
    public interface IRepositoryAsync<T, TKey, TUnsaved> : IRepositoryCreateAsync<T>, IRepositoryMergeAsync<T, TUnsaved>, IRepositoryReadAsync<T, TKey>, IRepositoryUpdateAsync<T>, IRepositoryDeleteAsync<T, TKey>, IRepositoryListAsync<T>
    {

    }
}