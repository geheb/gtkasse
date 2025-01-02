namespace GtKasse.Core.Repositories;

using GtKasse.Core.Converter;
using GtKasse.Core.Database;
using GtKasse.Core.Entities;
using GtKasse.Core.Extensions;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;

public sealed class WikiArticles
{
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly AppDbContext _dbContext;

    public WikiArticles(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasIdentifier(string identifier, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<WikiArticle>().AnyAsync(e => e.Identifier == identifier, cancellationToken);
    }

    public async Task<WikiArticleListDto[]> GetList(bool includeManagementBoard, CancellationToken cancellationToken)
    {      
        var entities = await _dbContext.Set<WikiArticle>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => includeManagementBoard || e.DescriptionMember != null)
            .OrderBy(e => e.Title)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new WikiArticleListDto(e, dc)).ToArray();
    }

    public async Task<WikiArticleDto?> Find(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<WikiArticle>()
            .AsNoTracking()
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null) return null;

        var dc = new GermanDateTimeConverter();

        return new WikiArticleDto(entity, dc);
    }

    public async Task<bool> Update(WikiArticleDto dto, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<WikiArticle>().FindAsync(new object[] { dto.Id }, cancellationToken);
        if (entity == null) return false;

        var count = 0;
        if (entity.SetValue(e => e.Identifier, dto.Identifier?.Trim())) count++;
        if (entity.SetValue(e => e.Title, dto.Title)) count++;
        if (entity.SetValue(e => e.UserId, dto.UserId)) count++;
        var descriptionMember = string.IsNullOrEmpty(dto.DescriptionMember?.Trim()) ? null : dto.DescriptionMember;
        if (entity.SetValue(e => e.DescriptionMember, descriptionMember)) count++;
        var descriptionManagementBoard = string.IsNullOrEmpty(dto.DescriptionManagementBoard?.Trim()) ? null : dto.DescriptionManagementBoard;
        if (entity.SetValue(e => e.DescriptionManagementBoard, descriptionManagementBoard)) count++;

        if (count < 1) return true;

        entity.UpdatedOn = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Create(WikiArticleDto dto, CancellationToken cancellationToken)
    {
        var entity = new WikiArticle
        {
            Id = _pkGenerator.Generate(),
            CreatedOn = DateTimeOffset.UtcNow,
            Identifier = dto.Identifier?.Trim(),
            Title = dto.Title,
            UserId = dto.UserId,
            DescriptionMember = string.IsNullOrEmpty(dto.DescriptionMember?.Trim()) ? null : dto.DescriptionMember,
            DescriptionManagementBoard = string.IsNullOrEmpty(dto.DescriptionManagementBoard?.Trim()) ? null : dto.DescriptionManagementBoard
        };

        await _dbContext.Set<WikiArticle>().AddAsync(entity, cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<WikiArticle>().FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return false;

        _dbContext.Set<WikiArticle>().Remove(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
