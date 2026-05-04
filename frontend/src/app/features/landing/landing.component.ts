import { Component, OnDestroy } from '@angular/core';
import { NgForOf } from '@angular/common';
import { RouterLink } from '@angular/router';

interface LandingSlide {
  title: string;
  description: string;
  video: string;
}

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterLink, NgForOf],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent implements OnDestroy {
  readonly slides: LandingSlide[] = [
    {
      title: 'Recipe Vault',
      description: 'Store all your family recipes with ingredients, cook time, and notes for every dish.',
      video: '/Recipes Feature.mp4'
    },
    {
      title: 'Tag Filtering',
      description: 'Filter recipes by tags like vegetarian, quick, and weekend so you find the perfect meal fast.',
      video: '/Tag Feature.mp4'
    },
    {
      title: 'Meal Plan',
      description: 'Create a weekly meal plan in seconds and follow a consistent cooking schedule.',
      video: '/MealPlan Feature.mp4'
    },
    {
      title: 'Shop for Groceries',
      description: 'Convert your meal plan into a grocery list and shop for everything in one place.',
      video: '/Shop Feature.mp4'
    }
  ];

  currentIndex = 0;
  private slideshowInterval?: number;

  constructor() {
    this.slideshowInterval = window.setInterval(() => this.nextSlide(), 21000);
  }

  get activeSlide(): LandingSlide {
    return this.slides[this.currentIndex];
  }

  nextSlide(): void {
    this.currentIndex = (this.currentIndex + 1) % this.slides.length;
  }

  prevSlide(): void {
    this.currentIndex = (this.currentIndex - 1 + this.slides.length) % this.slides.length;
  }

  goToSlide(index: number): void {
    this.currentIndex = index;
  }

  ngOnDestroy(): void {
    if (this.slideshowInterval !== undefined) {
      window.clearInterval(this.slideshowInterval);
    }
  }
}
