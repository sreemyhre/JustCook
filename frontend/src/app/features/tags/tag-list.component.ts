import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { TagService } from '../../core/services/tag.service';
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
  private snackBar = inject(MatSnackBar);

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
    this.tagService.getAll().subscribe({
      next: tags => this.tags.set(tags),
      error: () => this.showError('Failed to load tags')
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

    const duplicate = this.tags().some(t => t.id !== tag.id && t.name.toLowerCase() === name.toLowerCase());
    if (duplicate) {
      this.showError(`"${name}" already exists`);
      return;
    }

    this.tagService.update(tag.id, { name }).subscribe({
      next: updated => {
        this.tags.update(list => list.map(t => t.id === updated.id ? updated : t));
        this.editingId.set(null);
        this.showSuccess('Tag updated');
      },
      error: () => this.showError('Failed to update tag — please try again')
    });
  }

  confirmDelete(tag: TagDto): void {
    this.deletingId.set(tag.id);
    this.editingId.set(null); // close any open edit
  }

  cancelDelete(): void {
    this.deletingId.set(null);
  }

  deleteTag(tag: TagDto): void {
    this.tagService.delete(tag.id).subscribe({
      next: () => {
        this.tags.update(list => list.filter(t => t.id !== tag.id));
        this.deletingId.set(null);
        this.showSuccess('Tag deleted');
      },
      error: () => {
        this.deletingId.set(null);
        this.showError('Failed to delete tag');
      }
    });
  }

  addTag(): void {
    const name = this.newTagName().trim();
    if (!name) return;

    const duplicate = this.tags().some(t => t.name.toLowerCase() === name.toLowerCase());
    if (duplicate) {
      this.addTagError.set(`"${name}" already exists`);
      return;
    }

    this.addTagError.set('');
    this.tagService.create({ name }).subscribe({
      next: created => {
        this.tags.update(list => [...list, created]);
        this.newTagName.set('');
        this.showSuccess('Tag added');
      },
      error: () => this.showError('Failed to add tag — please try again')
    });
  }

  private showSuccess(msg: string): void {
    this.snackBar.open(msg, 'Close', { duration: 3000 });
  }

  private showError(msg: string): void {
    this.snackBar.open(msg, 'Close', { duration: 4000 });
  }
}
