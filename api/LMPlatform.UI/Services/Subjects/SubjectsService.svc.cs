﻿using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using System.Web.Http;
using Application.Core.Data;
using LMPlatform.Models;
using LMPlatform.UI.Services.Modules;
using LMPlatform.UI.Services.Modules.Parental;
using LMPlatform.UI.Attributes;
using WebMatrix.WebData;
using LMPlatform.UI.ViewModels.SubjectViewModels;
using System.Collections.Generic;
using Application.Infrastructure.GroupManagement;

namespace LMPlatform.UI.Services.Subjects
{
    [JwtAuth]
    public class SubjectsService : ISubjectsService
    {
        private readonly LazyDependency<ISubjectManagementService> subjectManagementService = new LazyDependency<ISubjectManagementService>();
        private readonly LazyDependency<IModulesManagementService> modulesManagementService = new LazyDependency<IModulesManagementService>();
        private readonly LazyDependency<IGroupManagementService> groupManagementService = new LazyDependency<IGroupManagementService>();
        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;
        public IModulesManagementService ModulesManagementService => modulesManagementService.Value;

        public IGroupManagementService GroupManagementService => groupManagementService.Value;

        public SubjectsResult GetSubjectsBySession()
        {
            var subjects = SubjectManagementService.GetUserSubjectsV2(WebSecurity.CurrentUserId);

            var result = new SubjectsResult
            {
                Subjects = subjects.Select(e => new SubjectViewData(e)).ToList()
            };

            return result;
        }

        public SubjectResult Update(SubjectViewData subject)
        {
	        var query = new Query<Subject>(s => s.Id == subject.Id)
		        .Include(s => s.SubjectGroups)
		        .Include(s => s.SubjectModules);
            var loadedSubject = SubjectManagementService.GetSubject(query);
            loadedSubject.IsNeededCopyToBts = subject.IsNeededCopyToBts;
            SubjectManagementService.SaveSubject(loadedSubject);
            return new SubjectResult
            {
                Subject = new SubjectViewData(loadedSubject)
            };
        }

        public IEnumerable<ModulesViewModel> GetSubjectModules(string subjectId)
        {
            var modules = ModulesManagementService.GetModules(int.Parse(subjectId))
                .Where(e => e.Visible)
                .Select(m => new ModulesViewModel(m, true)).ToList();

            return modules.OrderBy(m => m.Order);
        }
    }
}
