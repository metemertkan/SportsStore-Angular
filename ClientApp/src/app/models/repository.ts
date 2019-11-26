import { Product } from "./product.model";

export class Repository {
    product: Product;

    constructor() {
        this.product = JSON.parse(document.getElementById("data").textContent);
    }
}