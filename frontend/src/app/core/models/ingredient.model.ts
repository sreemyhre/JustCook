export interface IngredientDto {
  id: number;
  recipeId: number;
  name: string;
  quantity: number;
  unit: string;
  isStaple: boolean;
}

export interface CreateIngredientDto {
  name: string;
  quantity: number;
  unit: string;
  isStaple: boolean;
}
