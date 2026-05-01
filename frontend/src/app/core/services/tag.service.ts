import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TagDto, CreateTagDto, UpdateTagDto } from '../models/tag.model';

@Injectable({ providedIn: 'root' })
export class TagService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/tags`;
  private cache$: Observable<TagDto[]> | null = null;

  getAll(): Observable<TagDto[]> {
    if (!this.cache$) {
      this.cache$ = this.http.get<TagDto[]>(this.base).pipe(shareReplay(1));
    }
    return this.cache$;
  }

  create(dto: CreateTagDto): Observable<TagDto> {
    return this.http.post<TagDto>(this.base, dto).pipe(
      tap(() => this.clearCache())
    );
  }

  update(id: number, dto: UpdateTagDto): Observable<TagDto> {
    return this.http.put<TagDto>(`${this.base}/${id}`, dto).pipe(
      tap(() => this.clearCache())
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`).pipe(
      tap(() => this.clearCache())
    );
  }

  private clearCache(): void {
    this.cache$ = null;
  }
}
