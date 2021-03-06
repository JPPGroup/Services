﻿using AutoMapper;
using System;
using System.Linq;
using Jpp.Projects.Models;
using Jpp.Projects.Resources;

namespace Jpp.Projects.Mappings
{
    public class ModelToResourceProfile : Profile
    {
        private const double SPACING = 500;

        public ModelToResourceProfile()
        {
            CreateMap<Project, ProjectResource>()
                .ForMember(r => r.Folder, map => map.MapFrom(m => $"{m.Code}-{m.Name}"))
                .ForMember(r => r.Category, map => map.MapFrom(m => ((Category)m.Category).ToDescription()))
                .AfterMap(BuildFolderProperties);
        }

        private static void BuildFolderProperties(Project source, ProjectResource destination)
        {
            var baseFolder = GetBaseFolder(source.Code);
            var grouping = GetGrouping(source.Code);

            destination.Grouping = grouping;
            destination.SharedMailPath = $@"{baseFolder}\{grouping}\{destination.Folder}";
        }

        private static string GetBaseFolder(string projectCode)
        {
            var code = new string(projectCode.Where(char.IsDigit).ToArray());

            if (!int.TryParse(code, out var projectNum)) return "JPP_Shared";

            return projectNum >= 20000 ? "JPP_20000" : "JPP_Shared";
        }

        private static string GetGrouping(string projectCode)
        {
            return IsDevCode(projectCode, out var codeDev) ? codeDev : StandardJobCode(projectCode);
        }

        private static bool IsDevCode(string projectCode, out string groupFolder)
        {
            if (projectCode == "1000P" || projectCode == "1001P" || projectCode == "1002P" || projectCode == "1003P" || projectCode == "1005P")
            {
                groupFolder = "Software Development";
                return true;
            }

            groupFolder = "";
            return false;
        }

        private static string StandardJobCode(string projectCode)
        {
            var code = new string(projectCode.Where(char.IsDigit).ToArray());

            if (!double.TryParse(code, out var job)) return "Miscellaneous";

            var lower = Math.Floor(job / SPACING) * SPACING;
            var upper = lower + (SPACING - 1);

            return $"{lower}-{upper}";
        }
    }
}
