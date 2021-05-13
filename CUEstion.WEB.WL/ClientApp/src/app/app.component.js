var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Component } from '@angular/core';
let AppComponent = class AppComponent {
    constructor() {
        this.name = '';
    }
};
AppComponent = __decorate([
    Component({
        selector: 'app',
        template: `<label>Введите имя:</label>
                 <input [(ngModel)]="name" placeholder="name">
                 <h2>Добро пожаловать {{name}}!</h2>`
    })
], AppComponent);
export { AppComponent };
//# sourceMappingURL=app.component.js.map