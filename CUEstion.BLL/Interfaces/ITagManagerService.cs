﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CUEstion.BLL.Interfaces
{
    public interface ITagManagerService
    {
        public Task<IEnumerable<String>> GetAllTags();
        public Task CreateTag(string tag);
        public Task UpdateTag(string oldTag, string newTag);
        public Task DeleteTag(string tag);
    }
}