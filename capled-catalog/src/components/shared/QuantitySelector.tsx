import React from 'react';

type QuantitySelectorProps = {
  quantity: number;
  onChange: (newQuantity: number) => void;
  min?: number;
  max?: number;
};

export const QuantitySelector = ({ quantity, onChange, min = 1, max = 99 }: QuantitySelectorProps) => {
  return (
    <div className="input-group" style={{ width: '120px' }}>
      <button 
        className="btn btn-outline-secondary" 
        type="button" 
        onClick={() => quantity > min && onChange(quantity - 1)}
        disabled={quantity <= min}
      >
        <i className="bi bi-dash"></i>
      </button>
      <input 
        type="number" 
        className="form-control text-center px-1" 
        min={min} 
        max={max}
        value={quantity}
        onChange={(e) => {
            const val = parseInt(e.target.value, 10);
            if (!isNaN(val) && val >= min && val <= max) onChange(val);
        }}
      />
      <button 
        className="btn btn-outline-secondary" 
        type="button" 
        onClick={() => quantity < max && onChange(quantity + 1)}
        disabled={quantity >= max}
      >
        <i className="bi bi-plus"></i>
      </button>
    </div>
  );
};
