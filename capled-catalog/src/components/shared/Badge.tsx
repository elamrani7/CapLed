import React from 'react';

type BadgeProps = {
  text: string;
  type: 'success' | 'warning' | 'danger' | 'info' | 'secondary';
};

export const Badge = ({ text, type }: BadgeProps) => {
  return (
    <span className={`badge bg-${type} rounded-pill`}>
      {text}
    </span>
  );
};

export const BadgeDisponibilite = ({ dispo }: { dispo: string }) => {
  switch (dispo?.toUpperCase()) {
    case 'EN_STOCK':
      return <Badge text="En Stock" type="success" />;
    case 'STOCK_LIMITE':
      return <Badge text="Stock Limité" type="warning" />;
    case 'SUR_COMMANDE':
      return <Badge text="Sur Commande" type="info" />;
    case 'INDISPONIBLE':
    case 'RUPTURE':
      return <Badge text="Indisponible" type="danger" />;
    default:
      return <Badge text={dispo || 'Non spécifié'} type="secondary" />;
  }
};

export const BadgeCondition = ({ condition, grade }: { condition: string; grade?: string }) => {
  let text = condition;
  if (condition?.toUpperCase() === 'RECONDITIONNE' && grade) {
    text += ` (Grade ${grade})`;
  }

  switch (condition?.toUpperCase()) {
    case 'NEUF':
      return <span className="badge text-bg-success rounded-pill">Neuf</span>;
    case 'OCCASION':
      return <span className="badge text-bg-warning rounded-pill">Occasion</span>;
    case 'RECONDITIONNE':
      return <span className="badge text-bg-info rounded-pill">Reconditionné</span>;
    case 'EN_REPARATION':
      return <span className="badge text-bg-secondary rounded-pill">En Réparation</span>;
    case 'ENDOMMAGE':
      return <span className="badge text-bg-danger rounded-pill">Endommagé</span>;
    default:
      return <span className="badge bg-secondary rounded-pill">{text || 'ND'}</span>;
  }
};
