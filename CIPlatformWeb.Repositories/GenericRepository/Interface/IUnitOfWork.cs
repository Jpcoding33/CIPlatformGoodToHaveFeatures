using CIPlatformWeb.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Repositories.GenericRepository.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        ICityRepository City { get; }
        ICountryRepository Country { get; }
        ICommentRepository Comment { get; }
        IFavoriteMissionRepository FavoriteMission { get; }
        IMissionApplicationRepository MissionApplication { get; }
        IMissionInviteRepository MissionInvite { get; }
        IMissionRepository Mission { get; }
        IMissionThemeRepository MissionTheme { get; }
        IPasswordResetRepository PasswordReset { get; }
        IStoryMediaRepository StoryMedia { get; }
        IStoryRepository Story { get; }
        IUserRepository User { get; }
        IMissionRatingRepository MissionRating { get; }
        IStoryInviteRepository StoryInvite { get; }
        ISkillRepository Skill { get; }
        IUserSkillRepository UserSkill { get; }
        ITimeSheetRepository TimeSheet { get; }
        ICMSPageRepository CMSPage { get; }
        IMissionSkillRepository MissionSkill { get; }
        IBannerRepository Banner { get; }
        IGoalMisisonRepository GoalMisison { get; }
        IMissionMediaRepository MissionMedia { get; }
        IMissionDocumentsRepository MissionDocuments { get; }
        IContactUsRepository ContactUs { get; }
        INotifTypeRepository NotifType { get; }
        INotificationToAll NotifToAll { get; }
        int Save();
    }
}
