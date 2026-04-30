import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { RecipeDto } from '../../../core/models/recipe.model';
import { CalendarWeek, DropEvent } from '../planner.types';
import { formatMonthYear } from '../planner.utils';

const DAY_HEADERS = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

@Component({
  selector: 'app-planner-calendar',
  standalone: true,
  imports: [
    CommonModule,
    DragDropModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './planner-calendar.component.html',
  styleUrl: './planner-calendar.component.scss'
})
export class PlannerCalendarComponent {
  @Input() weeks: CalendarWeek[] = [];
  @Input() viewMonth: Date = new Date();
  @Input() editMode = false;
  @Input() savingDay: string | null = null;

  @Output() drop = new EventEmitter<DropEvent>();
  @Output() prevMonth = new EventEmitter<void>();
  @Output() nextMonth = new EventEmitter<void>();
  @Output() toggleEdit = new EventEmitter<void>();

  readonly dayHeaders = DAY_HEADERS;

  private readonly _today = (() => {
    const d = new Date();
    return { y: d.getFullYear(), m: d.getMonth(), d: d.getDate() };
  })();

  get hasFutureWeeks(): boolean {
    return this.weeks.some(w => w.isFuture);
  }

  formatMonthYear(date: Date): string {
    return formatMonthYear(date);
  }

  isToday(date: Date): boolean {
    return date.getDate() === this._today.d &&
           date.getMonth() === this._today.m &&
           date.getFullYear() === this._today.y;
  }

  isSavingDay(weekKey: string, dayOfWeek: number): boolean {
    return this.savingDay === `${weekKey}-${dayOfWeek}`;
  }

  isOutOfMonth(date: Date): boolean {
    return date.getMonth() !== this.viewMonth.getMonth() ||
           date.getFullYear() !== this.viewMonth.getFullYear();
  }

  onCellDrop(event: CdkDragDrop<any>, week: CalendarWeek, dayOfWeek: number): void {
    if (event.previousContainer === event.container) return;
    if (!week.isFuture) return;
    const recipe: RecipeDto = event.item.data;
    this.drop.emit({ week, dayOfWeek, recipe });
  }
}
