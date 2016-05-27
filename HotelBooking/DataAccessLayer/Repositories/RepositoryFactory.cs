using Common.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TE.Common.Logging;

namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// Creates repositories
    /// </summary>
    /// <remarks>Instances of repositories are not thread safe - you must call for a repository
    /// for each thread and every time you do an impersonation.</remarks>
    public class RepositoryFactory
    {
        public enum GetActionTypes
        {
            ThrowsUnauthorizedException,
            ReturnsNull
        }

        private class Filter
        {
            public object FilterExpression { get; set; }

            public Func<TEIdentityBase, bool> AppliesWhen { get; set; }

            public GetActionTypes Action { get; set; }
        }

        public static readonly Func<TEIdentityBase, bool> AppliesToAllFunc = (i) => true;

        private static volatile Dictionary<Type, List<Filter>> filters;

        private static object syncRoot = new Object();

        static RepositoryFactory()
        {
            lock (syncRoot)
            {
                Reset();
            }
        }



        public static TRepository Get<TRepository, TEntity>(bool autoDetectChangesEnabled = true)
            where TRepository : Repository<TEntity>
            where TEntity : BaseModel
        {
            Logger.InstanceVerbose.LogFunctionEntry(typeof(RepositoryFactory).Name, String.Format("Repository<{0}>", typeof(TEntity).Name));

            Expression<Func<TEntity, bool>> filter;
            String username;
            bool throwsUnauthorized;

            if (filters.ContainsKey(typeof(TEntity)))
            {
                GetActionTypes action;
                filter = GetFilter<TEntity>(out action);
                username = Thread.CurrentPrincipal.Identity.Name;
                throwsUnauthorized = action == GetActionTypes.ThrowsUnauthorizedException;
            }
            else
            {
                filter = null;
                username = null;
                throwsUnauthorized = true;
            }

            Object repo;

            if (typeof(TRepository) == typeof(Repository<TEntity>))
            {
                repo = new Repository<TEntity>(username, filter, throwsUnauthorized, autoDetectChangesEnabled);
            }
            else if (typeof(TRepository) == typeof(QuoteRepository))
            {
                repo = new QuoteRepository(username, filter as Expression<Func<Models.Quote, bool>>, throwsUnauthorized, autoDetectChangesEnabled); // Add same params as below.
            }
            else
            {
                throw new NotImplementedException();
            }

            Logger.InstanceVerbose.LogFunctionExit(typeof(RepositoryFactory).Name, String.Format("Repository<{0}>", typeof(TEntity).Name));

            return repo as TRepository;
        }


        /// <summary>
        /// This will return a repository with the correct filters applied.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static Repository<TEntity> Get<TEntity>(bool autoDetectChangesEnabled = true) where TEntity : BaseModel
        {
            Logger.InstanceVerbose.LogFunctionEntry(typeof(RepositoryFactory).Name, String.Format("Repository<{0}>", typeof(TEntity).Name));

            var entityRepository = Get<Repository<TEntity>, TEntity>(autoDetectChangesEnabled);

            Logger.InstanceVerbose.LogFunctionExit(typeof(RepositoryFactory).Name, String.Format("Repository<{0}>", typeof(TEntity).Name));

            return entityRepository;
        }

        /// <summary>
        /// DO NOT USE. Required for initial login only.
        /// </summary>
        public static Repository<TEntity> GetUnsecured<TEntity>() where TEntity : BaseModel
        {
            return new Repository<TEntity>(null, null);
        }

        /// <summary>
        /// This will register the given filter in the repository. Until Reset() is called, this filter will
        /// always be applied when "appliesWhen" is true.
        /// </summary>
        /// <typeparam name="TEntity">The entity to apply the filter too. Note that this does not affect navigation properties (at this time)</typeparam>
        /// <param name="filter">The filter expression. Will be used in the LINQ to SQL query</param>
        /// <param name="appliesWhen">The filter is applied when this is true. This need not be convertable to SQL.</param>
        /// <param name="action">Dictates the behaviour that any get by ID methods undertake</param>
        /// <remarks>For both functions above, you should not save state between threads and users.
        /// Additionaly, the appliesWhen should be consistent  per authenticated user(TEIdentityBase) and will be called at least once, but maybe only once (with outcome cached).
        /// </remarks>
        public static void RegisterFilter<TEntity, TIdentity>(Expression<Func<TEntity, bool>> filter, Func<TIdentity, bool> appliesWhen = null, GetActionTypes action = GetActionTypes.ThrowsUnauthorizedException)
            where TEntity : BaseModel
            where TIdentity : TEIdentityBase
        {
            Logger.InstanceVerbose.LogFunctionEntry(typeof(RepositoryFactory).Name, String.Format("RegisterFilter<{0}>", typeof(TEntity).Name));

            Logger.InstanceVerbose.Debug(typeof(RepositoryFactory).Name, String.Format("RegisterFilter<{0}>", typeof(TEntity).Name),
                "Registering a filter for type " + typeof(TEntity) + " with action " + action);
            lock (syncRoot)
            {
                var appliesWhenToUse = appliesWhen ?? AppliesToAllFunc;

                if (!filters.ContainsKey(typeof(TEntity)))
                {
                    var list = new List<Filter>
                    {
                        new Filter {FilterExpression = filter, AppliesWhen = t => t is TIdentity && appliesWhenToUse(t as TIdentity), Action = action}
                    };

                    filters.Add(typeof(TEntity), list);
                }
                else
                {
                    var list = filters[typeof(TEntity)];

                    // Yes - this is a reference check on the delegate so we can concat the filters properly
                    var match = list.SingleOrDefault(d => d.AppliesWhen == appliesWhenToUse && d.Action == action);

                    // We need to add it to the list as there is a different appliesWhen property
                    if (match == null)
                        list.Add(new Filter { FilterExpression = filter, AppliesWhen = t => t is TIdentity && appliesWhenToUse(t as TIdentity), Action = action });
                    else
                    {
                        // Concat it to the previous expression
                        var expression = (Expression<Func<TEntity, bool>>)match.FilterExpression;
                        match.FilterExpression = expression.And(filter);
                    }
                }
            }

            Logger.InstanceVerbose.LogFunctionExit(typeof(RepositoryFactory).Name, String.Format("RegisterFilter<{0}>", typeof(TEntity).Name));
        }

        /// <summary>
        /// Returns a filter contexualised to the logged on user and entity.
        /// </summary>
        private static Expression<Func<TEntity, bool>> GetFilter<TEntity>(out GetActionTypes actionType) where TEntity : BaseModel
        {
            Logger.InstanceVerbose.LogFunctionEntry(typeof(RepositoryFactory).Name, String.Format("GetFilter<{0}>", typeof(TEntity).Name));

            Expression<Func<TEntity, bool>> filters;

            if (Thread.CurrentPrincipal.Identity as TEIdentityBase == null)
            {
                // Disallow access completely
                actionType = GetActionTypes.ThrowsUnauthorizedException;
                return t => false;
                //throw new UnauthorizedException("A repository can not be created for an anon user");
            }

            // Based on the appliesWhen outcome, concat all applicable filters and return
            var applicableFilters =
                RepositoryFactory.filters[typeof(TEntity)].Where(t => t.AppliesWhen(Thread.CurrentPrincipal.Identity as TEIdentityBase)).ToList();

            actionType = GetActionTypes.ReturnsNull;

            if (applicableFilters.Any())
            {
                if (applicableFilters.Any(t => t.Action == GetActionTypes.ThrowsUnauthorizedException))
                {
                    actionType = GetActionTypes.ThrowsUnauthorizedException;
                }

                filters = applicableFilters.Select(t => t.FilterExpression as Expression<Func<TEntity, bool>>).Aggregate((a, b) => a.And(b));
            }
            else
            {
                filters = null;
            }

            // Log outcome
            var message = "Applicable Filters Found? = " + (filters != null);
            if (filters != null)
                message += "(" + actionType + ")";

            Logger.InstanceVerbose.Info(typeof(RepositoryFactory).Name, String.Format("GetFilter<{0}>", typeof(TEntity).Name), message);

            Logger.InstanceVerbose.LogFunctionExit(typeof(RepositoryFactory).Name, String.Format("GetFilter<{0}>", typeof(TEntity).Name));

            return filters;
        }

        /// <summary>
        /// This clears all filters
        /// </summary>
        public static void Reset()
        {
            Logger.InstanceVerbose.LogFunctionEntry(typeof(RepositoryFactory).Name, "Reset");

            lock (syncRoot)
            {
                filters = new Dictionary<Type, List<Filter>>();
            }

            Logger.InstanceVerbose.LogFunctionExit(typeof(RepositoryFactory).Name, "Reset");
        }
    }
}
