import { ApiService } from './../../shared/services/api.service';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BookCategory } from '../../models/models';

@Component({
  selector: 'addbooks',
  templateUrl: './addbooks.component.html',
  styleUrl: './addbooks.component.scss'
})
export class AddbooksComponent {
  category: BookCategory[] = [];
  addBookFrom: FormGroup;
  constructor(
    private fb: FormBuilder,
    private apiService : ApiService,
    private snackBar: MatSnackBar
  ) {
    apiService.getCategories().subscribe({
      next: (res: BookCategory[]) => {
        this.category = [];
        res.forEach((b) => this.category.push(b));
      }
    })
    this.addBookFrom = fb.group({
      title: fb.control('', [Validators.required]),
      author: fb.control('', [Validators.required]),
      price: fb.control('', [Validators.required]),
      category: fb.control('', [Validators.required]),
    });

  }
  addbooks(){
   let books = {
      title: this.addBookFrom.get('title')?.value,
      author: this.addBookFrom.get('author')?.value,
      price: this.addBookFrom.get('price')?.value,
      bookCategoryId: this.addBookFrom.get('category')?.value,
      ordered: true,
   };
   this.apiService.addBook(books).subscribe({
    next: (res) => {
      this.snackBar.open(res, 'OK');
      this.addBookFrom.reset();
    },
  });
  }
}
