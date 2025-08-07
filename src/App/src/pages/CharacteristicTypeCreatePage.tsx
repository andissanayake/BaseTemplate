import { Card } from "antd";
import { CharacteristicTypeCreate } from "../features/CharacteristicType/CharacteristicTypeCreate";

const CharacteristicTypeCreatePage: React.FC = () => {
  return (
    <Card>
      <CharacteristicTypeCreate />
    </Card>
  );
};

export { CharacteristicTypeCreatePage };
