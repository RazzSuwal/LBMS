import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray, FormControl } from '@angular/forms';
import { ApiService } from '../../shared/services/api.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'billing',
  templateUrl: './billing.component.html',
  styleUrls: ['./billing.component.scss']
})
export class BillingComponent {
  createBillingForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private snackBar: MatSnackBar
  ) {
    this.createBillingForm = this.fb.group({
      documentNo: ['', Validators.required],
      customerName: ['', Validators.required],
      mobileNumber: ['', Validators.required],
      discount: [0, Validators.required],
      grandTotal: [0, Validators.required],
      products: this.fb.array([]) // Initialize FormArray for products
    });
  }

  // Function to add a new product row
  addProduct() {
    const productGroup = this.fb.group({
      productName: ['', Validators.required],
      quantity: [0, Validators.required],
      rate: [0, Validators.required],
      amount: [0, Validators.required]
    });
    this.products.push(productGroup);
  }

  // Function to remove a product row
  removeProduct(index: number) {
    this.products.removeAt(index);
  }

  // Getter for products FormArray
  get products() {
    return this.createBillingForm.get('products') as FormArray;
  }

  // Function to submit billing
  addBilling() {
    if (this.createBillingForm.valid) {
      const billingData = this.createBillingForm.value;
      // Make API call to submit the billing information
      this.apiService.createBilling(billingData).subscribe({
        next: (res) => {
          this.snackBar.open('Billing submitted successfully!', 'OK', { duration: 3000 });
          this.createBillingForm.reset(); // Reset the form after successful submission
          this.products.clear(); // Clear products FormArray after submission
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err}`, 'OK', { duration: 3000 });
        }
      });
    } else {
      this.snackBar.open('Please fill in all required fields.', 'OK', { duration: 3000 });
    }
  }
}
