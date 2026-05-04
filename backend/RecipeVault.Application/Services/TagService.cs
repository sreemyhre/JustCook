using AutoMapper;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;
using RecipeVault.Core.Entities;
using RecipeVault.Core.Interfaces;

namespace RecipeVault.Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public TagService(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<TagDto> CreateTagAsync(CreateTagDto dto)
    {
        var tag = _mapper.Map<Tag>(dto);
        var created = await _tagRepository.AddAsync(tag);
        return _mapper.Map<TagDto>(created);
    }

    public async Task<TagDto?> GetByIdAsync(int id, int userId)
    {
        var tag = await _tagRepository.GetByIdAsync(id, userId);
        if (tag == null) return null;
        return _mapper.Map<TagDto>(tag);
    }

    public async Task<IEnumerable<TagDto>> GetAllByUserIdAsync(int userId)
    {
        var tags = await _tagRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto?> UpdateTagAsync(int id, int userId, UpdateTagDto dto)
    {
        var tag = await _tagRepository.GetByIdAsync(id, userId);
        if (tag == null) return null;

        _mapper.Map(dto, tag);
        await _tagRepository.UpdateAsync(tag);
        return _mapper.Map<TagDto>(tag);
    }

    public async Task<bool> DeleteTagAsync(int id, int userId)
    {
        return await _tagRepository.DeleteAsync(id, userId);
    }

    public async Task<bool> AddTagToRecipeAsync(int recipeId, int tagId)
    {
        return await _tagRepository.AddTagToRecipeAsync(recipeId, tagId);
    }

    public async Task<bool> RemoveTagFromRecipeAsync(int recipeId, int tagId)
    {
        return await _tagRepository.RemoveTagFromRecipeAsync(recipeId, tagId);
    }

    public async Task<IEnumerable<RecipeDto>> GetRecipesByTagAsync(int tagId, int userId)
    {
        var recipes = await _tagRepository.GetRecipesByTagIdAsync(tagId, userId);
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }
}
