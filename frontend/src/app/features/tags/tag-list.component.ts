import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { TagService } from '../../core/services/tag.service';
import { ToastService } from '../../core/services/toast.service';
import { TagDto } from '../../core/models/tag.model';

@Component({
  selector: 'app-tag-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSnackBarModule
  ],
  templateUrl: './tag-list.component.html',
  styleUrl: './tag-list.component.scss'
})
export class TagListComponent implements OnInit {
  private tagService = inject(TagService);
  private toast = inject(ToastService);
  private destroyRef = inject(DestroyRef);

  tags = signal<TagDto[]>([]);
  displayedColumns = ['name', 'actions'];
  editingId = signal<number | null>(null);
  editingName = signal<string>('');
  deletingId = signal<number | null>(null);
  newTagName = signal<string>('');
  addTagError = signal<string>('');

  ngOnInit(): void {
    this.loadTags();
  }

  loadTags(): void {
    this.tagService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: tags => this.tags.set(tags),
        error: () => this.toast.error('Failed to load tags')
      });
  }

  startEdit(tag: TagDto): void {
    this.editingId.set(tag.id);
    this.editingName.set(tag.name);
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.editingName.set('');
  }

  saveEdit(tag: TagDto): void {
    const name = this.editingName().trim();
    if (!name) return;
    if (this.isDuplicateName(name, tag.id)) {
      this.toast.error(`"${name}" already exists`);
      return;
    }
    this.tagService.update(tag.id, { name })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: updated => {
          this.tags.update(list => list.map(t => t.id === updated.id ? updated : t));
          this.editingId.set(null);
          this.toast.success('Tag updated');
        },
        error: () => this.toast.error('Failed to update tag — please try again')
      });
  }

  confirmDelete(tag: TagDto): void {
    this.deletingId.set(tag.id);
    this.editingId.set(null);
  }

  cancelDelete(): void {
    this.deletingId.set(null);
  }

  deleteTag(tag: TagDto): void {
    this.tagService.delete(tag.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.tags.update(list => list.filter(t => t.id !== tag.id));
          this.deletingId.set(null);
          this.toast.success('Tag deleted');
        },
        error: () => {
          this.deletingId.set(null);
          this.toast.error('Failed to delete tag');
        }
      });
  }

  addTag(): void {
    const name = this.newTagName().trim();
    if (!name) return;
    if (this.isDuplicateName(name)) {
      this.addTagError.set(`"${name}" already exists`);
      return;
    }
    this.addTagError.set('');
    this.tagService.create({ name })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: created => {
          this.tags.update(list => [...list, created]);
          this.newTagName.set('');
          this.toast.success('Tag added');
        },
        error: () => this.toast.error('Failed to add tag — please try again')
      });
  }

  private isDuplicateName(name: string, excludeId?: number): boolean {
    return this.tags().some(t =>
      t.name.toLowerCase() === name.toLowerCase() && t.id !== excludeId
    );
  }
}
