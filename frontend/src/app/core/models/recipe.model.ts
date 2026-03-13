import { IngredientDto, CreateIngredientDto } from './ingredient.model';
import { TagDto } from './tag.model';

export interface RecipeDto {
  id: number;
  userId: number;
  name: string;
  description: string;
  servingSize: number;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  instructions: string;
  imageUrl: string;
  lastCookedDate: string | null;
  cookCount: number;
  createdAt: string;
  ingredients: IngredientDto[];
  tags: TagDto[];
}

export interface CreateRecipeDto {
  userId: number;
  name: string;
  description: string;
  servingSize: number;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  instructions: string;
  imageUrl: string;
  ingredients: CreateIngredientDto[];
  tagIds: number[];
}

export interface UpdateRecipeDto {
  name: string;
  description: string;
  servingSize: number;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  instructions: string;
  imageUrl: string;
  ingredients: CreateIngredientDto[];
  tagIds: number[];
}
